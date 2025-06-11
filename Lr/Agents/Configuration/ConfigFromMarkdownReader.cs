using Serilog;
#pragma warning disable SKEXP0070
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Markdig.Renderers.Normalize;
using Markdig.Syntax;
using Microsoft.SemanticKernel.Connectors.Ollama;

namespace Lr.Agents.Configuration;

public class ConfigFromMarkdownReader(string directory) : IAgentConfigurationFactory
{
    record ConfigRaw(
        string? Name,
        string? Prompt,
        string? SearchPrompt,
        Dictionary<string, string>? Parameters,
        string? Workflow);

    public async Task<InitResult[]> Init(CancellationToken ct)
    {
        var directoryInfo = new DirectoryInfo(directory);
        Logger.Information("Starting to read configuration files from directory: {Directory}", directory);

        // Parse                                                                                                    
        ConfigRaw? baseRaw = null;
        var raws = new List<ConfigRaw>();
        foreach (var fileInfo in directoryInfo.GetFiles("*.md"))
        {
            try
            {
                var config = await Read(fileInfo.FullName, ct);
                Logger.Debug("Parse configuration file: {Path}", fileInfo.FullName);

                if (fileInfo.Name == "base.md")
                {
                    baseRaw = config with { Workflow = "simple" };
                }
                else
                {
                    raws.Add(config);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to read configuration file: {Path}", fileInfo.FullName);
            }
        }

        //Parse                                                                                                    
        var result = new List<InitResult>(raws.Count);
        foreach (var raw in raws)
        {
            if (raw.Name == null)
            {
                result.Add(new InitResult.Failed("Name is required"));
                continue;
            }

            var prompt = raw.Prompt ?? baseRaw?.Prompt;
            if (prompt == null)
            {
                result.Add(new InitResult.Failed("Prompt is required"));
                continue;
            }

            var searchPrompt = raw.SearchPrompt ?? baseRaw?.SearchPrompt;
            if (searchPrompt == null)
            {
                result.Add(new InitResult.Failed("SearchPrompt is required"));
                continue;
            }

            var workflow = raw.Workflow ?? baseRaw?.Workflow;
            if (workflow == null)
            {
                result.Add(new InitResult.Failed("Workflow is required"));
                continue;
            }

            var parameters = raw.Parameters?.ToDictionary() ?? baseRaw?.Parameters?.ToDictionary();
            if (parameters == null)
            {
                result.Add(new InitResult.Failed("Parameters is required"));
                continue;
            }

            if (!parameters.TryGetValue("Model", out var modelName))
            {
                result.Add(new InitResult.Failed("Model is required"));
                continue;
            }

            var config = new AgentConfiguration()
            {
                Name = raw.Name,
                Prompt = prompt,
                SearchPrompt = searchPrompt,
                Workflow = workflow,
                ModelName = modelName,
                Parameters = Parse(parameters)
            };

            Logger.Debug("Finished read {@Config}", config);
            result.Add(new InitResult.Ok(config));
        }

        Logger.Information("Finished reading configuration files. Total configurations: {Count}", result.Count);
        return result.ToArray();
    }

    private async Task<ConfigRaw> Read(string path, CancellationToken ct)
    {
        var content = await File.ReadAllTextAsync(path, ct);
        var doc = Markdig.Markdown.Parse(content);
        var result = new ConfigRaw(null, null, null, null, null);

        if (TryGetName(doc, out var name))
        {
            result = result with { Name = name };
        }

        if (TryGetMainPrompt(doc, out var mainPrompt))
        {
            result = result with { Prompt = mainPrompt };
        }

        if (TryGetSearchPrompt(doc, out var searchPrompt))
        {
            result = result with { SearchPrompt = searchPrompt };
        }

        if (TryGetParameters(doc, out var parameters))
        {
            result = result with { Parameters = parameters };
        }

        Logger.Debug("{Path} md config read result {@Result}", path, result);
        return result;
    }


    private bool TryGetName(MarkdownDocument doc, [NotNullWhen(true)] out string? header)
    {
        var block = doc.FirstOrDefault(IsHeaderPredicate(1));
        
        if (block is not HeadingBlock headingBlock || headingBlock.Inline == null)
        {
            header = default;
            return false;
        }

        header = Render(headingBlock.Inline);
        return true;
    }

    private bool TryGetMainPrompt(MarkdownDocument doc, [NotNullWhen(true)] out string? prompt)
    {
        return TryGetPrompt(doc, "Prompt", out prompt);
    }

    private bool TryGetSearchPrompt(MarkdownDocument doc, [NotNullWhen(true)] out string? prompt)
    {
        return TryGetPrompt(doc, "Search", out prompt);
    }

    private bool TryGetParameters(MarkdownDocument doc,
        [NotNullWhen(true)] out Dictionary<string, string>? parameters)
    {
        if (TryGetSection(doc, "Model", out var blocks))
        {
            parameters = new Dictionary<string, string>();
            foreach (var block in blocks)
            {
                var blockPlainText = Render(block);
                var matches = Regex.Matches(blockPlainText, "`?(.+)?=(.+)?`");

                foreach (Match match in matches)
                {
                    var key = match.Groups[1].Value.Trim();
                    var value = match.Groups[2].Value.Trim();

                    if (!String.IsNullOrEmpty(key) || String.IsNullOrEmpty(value))
                    {
                        Logger.Debug("Parameters parse {Key}={Value}", key, value);
                        parameters[key] = value;
                    }
                    else
                    {
                        Logger.Debug("Parameters failed parse {Key}={Value}", key, value);
                    }
                }
            }

            return true;
        }

        parameters = null;
        return false;
    }

    private bool TryGetPrompt(MarkdownDocument doc, string section, [NotNullWhen(true)] out string? prompt)
    {
        if (!TryGetSection(doc, section, out var sectionBlocks))
        {
            prompt = default;
            return false;
        }

        var promptSw = new StringWriter();
        foreach (var block in sectionBlocks)
        {
            Render(promptSw, block);
        }

        prompt = promptSw.ToString();
        return true;
    }

    private bool TryGetSection(MarkdownDocument doc, string sectionName, [NotNullWhen(true)] out Block[]? blocks)
    {
        var headerBlock = doc
            .Where(IsHeaderPredicate(2))
            .Cast<HeadingBlock>()
            .Where(header => header.Inline != null)
            .Select(header => (Header: header, Text: Render(header.Inline!)))
            .FirstOrDefault(x => sectionName.Equals(x.Text)).Header;

        if (headerBlock == null)
        {
            blocks = null;
            return false;
        }

        blocks = doc
            .SkipWhile(x => x != headerBlock)
            .Skip(1)
            .TakeWhile(x => !IsHeader(2, x))
            .ToArray();
        return true;
    }

    private OllamaPromptExecutionSettings Parse(Dictionary<string, string>? p)
    {
        return new OllamaPromptExecutionSettings()
        {
            ModelId = p["Model"],
            TopK = PraseInt("TopK"),
            TopP = PraseFloat("TopP"),
            Temperature = PraseFloat("Temperature"),
            NumPredict = PraseInt("NumPredict"),
        };

        int? PraseInt(string name)
        {
            try
            {
                return int.Parse(p.GetValueOrDefault(name));
            }
            catch
            {
                return null;
            }
        }

        float? PraseFloat(string name)
        {
            try
            {
                return float.Parse(p.GetValueOrDefault(name));
            }
            catch
            {
                return null;
            }
        }
    }

    private string Render(MarkdownObject markdownObject)
    {
        using var sw = new StringWriter();
        var normRender = new NormalizeRenderer(sw);
        normRender.Render(markdownObject);
        return sw.ToString();
    }

    private void Render(TextWriter output, MarkdownObject markdownObject)
    {
        var normRender = new NormalizeRenderer(output);
        normRender.Render(markdownObject);
    }

    private static Func<Block, bool> IsHeaderPredicate(int level)
    {
        return block => IsHeader(level, block);
    }

    private static bool IsHeader(int level, Block block)
    {
        return block is HeadingBlock headerBlock && headerBlock.Level == level;
    }
}
