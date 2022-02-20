using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace Bastards
{
    internal class CommandLineCheats
    {
        [CommandLineFunctionality.CommandLineArgumentFunction("legitimize", "bastards")]
        private static string LegitimizeBastardCheat(List<string> args)
        {
            // bastards.legitimize Calrin Waters
            if (args.Count != 1)
            {
                return "Proper usage: bastards.legitimize BastardNameWithNoSpacesHere";
            }
            else
            {
                string givenName = args[0];

                foreach (Hero h in SubModule.BastardsManagerInstance.bastards.ToList())
                {
                    string nameWithNoSpace = h.Name.ToString().Replace(" ", "");
                    if (nameWithNoSpace.Equals(givenName))
                    {
                        Utils.LegitimizeBastard(h);
                        return h.Name.ToString() + " has been legitimized!";
                    }
                }

                return "There is no bastard with that name! Did you type the name without spaces?";
            }
        }
    }
}
