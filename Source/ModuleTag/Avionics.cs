namespace RP0.ModuleTags
{
    public class ModuleTagAvionics : ModuleTag
    {
        public override string GetInfo()
        {
<<<<<<< master
            return "Contains avionics to control the craft which requires extensive testing which increase the overall launch cost.\n\n" +
                   "<b><color=orange>Launch Cost: Cost of This Part * 3</color></b>";
=======
            return "Contains avionics to control the craft which requires extensive testing which increase the overall Rollout Cost.\n\n" +
    "<b><color=orange>Rollout Cost: Cost of This Part * 1.5</color></b>";
>>>>>>> Lower avionics kct multiplier to 1.5
        }
    }
}
