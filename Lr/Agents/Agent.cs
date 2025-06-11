#pragma warning disable SKEXP0070
using Lr.Integrations;
using Lr.Workflows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using OllamaSharp;

namespace Lr.Agents;

public class Agent(AgentConfiguration configuration)
{
    private IWorkflow? _workflow;

    public string Name => configuration.Name;
    public AgentConfiguration Configuration => configuration;
    public string? Description { get; set; }

    public async Task<IWorkflow> GetWorkflow(IServiceProvider sp)
    {
        Logger.Debug("Initializing workflow for agent: {AgentName}", Name);

        if (_workflow != null)
        {
            Logger.Debug("Returning existing workflow for agent: {AgentName}", Name);
            return _workflow;
        }

        if (configuration.Workflow == "simple")
        {
            Logger.Information("Creating SimpleWorkflow for agent: {AgentName}", Name);
            var kernel = CreateKernel(configuration.ModelName);
            var ollama = new Ollama(kernel, configuration.Parameters);
            var chat = new ChatContext(configuration.Prompt);
            _workflow = ActivatorUtilities.CreateInstance<SimpleWorkflow>(sp, this, chat, ollama);
            Logger.Information("SimpleWorkflow created successfully for agent: {AgentName}", Name);
            return _workflow;
        }

        throw new NotSupportedException($"Workflow type {configuration.Workflow} is not supported");
    }

    private static Kernel CreateKernel(string modelName)
    {
        Logger.Information("Creating Kernel with model: {ModelName}", modelName);
        try
        {
            var ollamaClient = new OllamaApiClient(new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:11434"),
                Timeout = TimeSpan.FromMinutes(10)
            });

            return Kernel
                .CreateBuilder()
                .AddOllamaEmbeddingGenerator(ollamaClient)
                .AddOllamaChatCompletion(ollamaClient)
                .AddOllamaTextGeneration(ollamaClient)
                .Build();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to create Kernel for model: {ModelName}", modelName);
            throw;
        }
    }
}
