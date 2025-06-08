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

    public async Task<IWorkflow> GetWorkflow(IServiceProvider sp)
    {
        if (_workflow != null)
        {
            return _workflow;
        }
        
        if (configuration.Workflow == "simple")
        {
            var kernel = CreateKernel(configuration.ModelName);
            var ollama = new Ollama(kernel, configuration.Parameters);
            var chat = new ChatContext(configuration.Prompt);
            _workflow = ActivatorUtilities.CreateInstance<SimpleWorkflow>(sp, this, chat, ollama);
            return _workflow;
        }

        throw new NotSupportedException();
    }

    private static Kernel CreateKernel(string modelName)
    {
        var ollamaClient = new OllamaApiClient(
            uriString: "http://localhost:11434",
            defaultModel: modelName
        );
       
        return Kernel
            .CreateBuilder()
            .AddOllamaEmbeddingGenerator(ollamaClient)
            .AddOllamaChatCompletion(ollamaClient)
            .AddOllamaTextGeneration(ollamaClient)
            .Build();
    }
}