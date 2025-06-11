using Lr.Agents;
using Lr.UI;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Lr.Workflows.Operations;

public class HelpOperation : IOperation
{
    public Task Execute(Agent agent, IWorkflow workflow, IUserInterface ui, ChatContext chat, string? query, CancellationToken ct)
    {
        var help = """
                   ## Search
                   1. **Стандартный поиск:**  
                      ```
                      : высота?
                      ```  
                      → Система проанализирует переписку и на основе заданного вопроса предложит поисковый запрос *Высота Эйфелевой башни*  
                   
                   2. **Ручной ввод:**  
                      ```
                      :! Как устроена солнечная система
                      ```  
                      → Поиск будет выполнен без генерации предложений *Как устроена солнечная система*  
                   
                   3. **Отмена:**  
                      ```
                      : 
                      (введите пустую строку при подтверждении)
                      ```  
                      → Система проанализирует переписку и предложит поисковый запрос *Высота Эйфелевой башни*
                   """;
        ui.PrintMessage(AuthorRole.Tool,  help);
        return Task.CompletedTask;
    }
}
