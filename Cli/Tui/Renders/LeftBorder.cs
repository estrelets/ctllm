using Spectre.Console;
using Spectre.Console.Rendering;

namespace Lr.UI.AnsiConsoleUi.Renders;

class LeftBorder : BoxBorder
{
    public override string GetPart(BoxBorderPart part)
    {
        if (part is BoxBorderPart.Left)
        {
            return "\u2502";
        }

        return " ";
    }
}
