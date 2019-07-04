using System;
using RP0.ProceduralAvionics;
<<<<<<< master
using RP0.Utilities;
=======
>>>>>>> ProcAvionicsTooling
using UnityEngine;

namespace RP0
{
    public class ModuleToolingProcAvionics : ModuleToolingPTank
    {
<<<<<<< master
        [KSPField]
        public float avionicsToolingCostMultiplier = 5f;

        public const string MainToolingType = "Avionics";
        private ModuleProceduralAvionics _procAvionics;

        public override string ToolingType => $"{MainToolingType}-{_procAvionics.CurrentProceduralAvionicsConfig.name[0]}{_procAvionics.CurrentProceduralAvionicsTechNode.techLevel}";
        private string TankToolingType => base.ToolingType;
        private ToolingDefinition TankToolingDefinition => ToolingManager.Instance.GetToolingDefinition(TankToolingType);

        private float ControllableMass => _procAvionics?.controllableMass ?? 0f;

        protected override void LoadPartModules()
        {
            base.LoadPartModules();
            _procAvionics = part.Modules.GetModule<ModuleProceduralAvionics>();
=======
        public const string MainToolingType = "Avionics";
        private ModuleProceduralAvionics procAvionics;

        public override string ToolingType => $"{MainToolingType}-{procAvionics.CurrentProceduralAvionicsConfig.name[0]}{procAvionics.CurrentProceduralAvionicsTechNode.techLevel}-{base.ToolingType}";

        protected override void LoadPartModules()
        {
            Debug.Log("[AvionicsTooling] Loading part modules");
            base.LoadPartModules();
            procAvionics = part.Modules.GetModule<ModuleProceduralAvionics>();
>>>>>>> ProcAvionicsTooling
        }

        public override string GetToolingParameterInfo()
        {
<<<<<<< master
<<<<<<< master
            return $"{Math.Round(ControllableMass, 3)} t x {base.GetToolingParameterInfo()}{GetInternalTankDiameterInfo()}";
        }

        private object GetInternalTankDiameterInfo()
        {
            if (_procAvionics.InternalTanksVolume == 0)
            {
                return "";
            }

            GetDimensions(out var diameter, out var length);
            var tankDiameter = GetInternalTankDiameter(diameter, length);
            return $" ({TankToolingType} {tankDiameter} m)";
=======
            return base.GetToolingParameterInfo() + $" x {Math.Round(ControllableMass, 3)} t";
>>>>>>> ProcAvionicsTooling
=======
            return $"{Math.Round(ControllableMass, 3)} t x {base.GetToolingParameterInfo()}";
>>>>>>> Use controlled mass first for tooling
        }

        public override float GetToolingCost()
        {
<<<<<<< master
            GetDimensions(out var diameter, out var length);
            return GetAvionicsToolingCost(diameter, length) + GetInternalTankToolingCost(diameter, length);
        }

        private float GetAvionicsToolingCost(float diameter, float length)
        {
            var toolingLevel = ToolingDatabase.GetToolingLevel(ToolingType, ControllableMass, diameter, length);
            var toolingCosts = GetPerLevelToolingCosts(diameter, length);

            return GetToolingCost(toolingLevel, toolingCosts);
        }

        private static float GetToolingCost(int toolingLevel, float[] toolingCosts)
        {
            var toolingCost = 0f;
            for (int i = toolingLevel; i < toolingCosts.Length; ++i)
            {
                toolingCost += toolingCosts[i];
            }

            return toolingCost;
        }

        private float[] GetPerLevelToolingCosts(float diameter, float length)
        {
            var controlledMassToolingFactor = 0.95f;
            var dimensionToolingFactor = 1 - controlledMassToolingFactor;
            return new[] {
                GetControlledMassToolingCost() * controlledMassToolingFactor,
                base.GetDiameterToolingCost(diameter) * dimensionToolingFactor,
                base.GetLengthToolingCost(diameter, length) * dimensionToolingFactor
            };
        }

        private float GetInternalTankToolingCost(float externalDiameter, float length)
        {
            if(_procAvionics.InternalTanksVolume == 0)
            {
                return 0;
            }

            var internalTankDiameter = GetInternalTankDiameter(externalDiameter, length);
            var level = ToolingDatabase.GetToolingLevel(TankToolingType, internalTankDiameter, internalTankDiameter);
            var perLevelCosts = new[] { GetDiameterToolingCost(internalTankDiameter), GetLengthToolingCost(internalTankDiameter, internalTankDiameter) };
            var costMult = TankToolingDefinition?.finalToolingCostMultiplier ?? 0f;
            return GetToolingCost(level, perLevelCosts) * costMult;
        }

        private float GetInternalTankDiameter(float externalDiameter, float length)
        {
            var maxDiameter = Mathf.Min(externalDiameter * 2 / 3, length);
            var internalTankDiameter = SphericalTankUtilities.GetSphericalTankRadius(_procAvionics.InternalTanksVolume) * 2;
            while (internalTankDiameter > maxDiameter) { internalTankDiameter /= 2; }

            return internalTankDiameter;
        }

        private float GetControlledMassToolingCost() => _procAvionics.GetModuleCost(0, ModifierStagingSituation.UNSTAGED) * avionicsToolingCostMultiplier;

        public override float GetModuleCost(float defaultCost, ModifierStagingSituation sit)
        {
            if (!onStartFinished) return 0f;

            return GetUntooledPenaltyCost() + GetInternalTankModuleCost();
        }

        private float GetInternalTankModuleCost()
        {
            if (_procAvionics.InternalTanksVolume == 0)
            {
                return 0;
            }

            GetDimensions(out var externalDiameter, out var length);
            var internalTankDiameter = GetInternalTankDiameter(externalDiameter, length);
            var tankCount = _procAvionics.InternalTanksVolume / SphericalTankUtilities.GetSphereVolume(internalTankDiameter / 2);
            var costMultDL = TankToolingDefinition?.costMultiplierDL ?? 0f;

            return GetDimensionModuleCost(internalTankDiameter, length, costMultDL) * tankCount;
=======
            GetDimensions(out var diameter, out var length, out var controllableMass);
            var toolingLevel = ToolingDatabase.GetToolingLevel(ToolingType, controllableMass, diameter, length);
            var avToolingFactor = 0.8f;
            var dimensionToolingFactor = 2 - avToolingFactor - procAvionics.Utilization;
            var toolingCosts = new[] { GetControlledMassToolingCost() * avToolingFactor, GetDiameterToolingCost(diameter) * dimensionToolingFactor, GetLengthToolingCost(diameter, length) * dimensionToolingFactor};
            var toolingCost = 0f;
            for(int i = toolingLevel; i < 3; ++i)
            {
                toolingCost += toolingCosts[i];
            }

<<<<<<< master
<<<<<<< master
            return (baseCost + procAvionics.GetModuleCost(0, ModifierStagingSituation.UNSTAGED) * 15) * finalToolingCostMultiplier;
>>>>>>> ProcAvionicsTooling
=======
            return toolingCost * 0.5f;
>>>>>>> Use controlled mass first for tooling
=======
            return toolingCost;
>>>>>>> Make dimension tooling cost based on utilization
        }

        private float GetControlledMassToolingCost() => procAvionics.GetModuleCost(0, ModifierStagingSituation.UNSTAGED) * 20;

        public override void PurchaseTooling()
        {
<<<<<<< master
            GetDimensions(out var diameter, out var length);
            ToolingDatabase.UnlockTooling(ToolingType, ControllableMass, diameter, length);
            UnlockInternalTankTooling(diameter, length);
        }

        private void UnlockInternalTankTooling(float diameter, float length)
        {
            if(_procAvionics.InternalTanksVolume == 0)
            {
                return;
            }

            var internalTankDiameter = GetInternalTankDiameter(diameter, length);
            ToolingDatabase.UnlockTooling(TankToolingType, internalTankDiameter, internalTankDiameter);
=======
            GetDimensions(out var diameter, out var length, out var controllableMass);
<<<<<<< master
            ToolingDatabase.UnlockTooling(ToolingType, diameter, length, controllableMass);
>>>>>>> ProcAvionicsTooling
=======
            ToolingDatabase.UnlockTooling(ToolingType, controllableMass, diameter, length);
>>>>>>> Use controlled mass first for tooling
        }

        public override bool IsUnlocked()
        {
<<<<<<< master
            if (_procAvionics == null)
            {
                return true;
            }

            GetDimensions(out var diameter, out var length);
            return IsAvionicsTooled(diameter, length) && IsInternalTankTooled(diameter, length);
        }

        private bool IsInternalTankTooled(float diameter, float length)
        {
            if(_procAvionics.InternalTanksVolume == 0)
            {
                return true;
            }

            var internalTankDiameter = GetInternalTankDiameter(diameter, length);
            return ToolingDatabase.GetToolingLevel(TankToolingType, internalTankDiameter, internalTankDiameter) == 2;
        }

        private bool IsAvionicsTooled(float diameter, float length) => ToolingDatabase.GetToolingLevel(ToolingType, ControllableMass, diameter, length) == 3;
=======
            if(procAvionics == null)
            {
                return true;
            }
            GetDimensions(out var diameter, out var length, out var controllableMass);
            return ToolingDatabase.GetToolingLevel(ToolingType, controllableMass, diameter, length) == 3;
        }

        private void GetDimensions(out float diameter, out float length, out float controllableMass)
        {
            if(procAvionics == null)
            {
                diameter = length = controllableMass = 0;
                return;
            }

            GetDimensions(out diameter, out length);
            controllableMass = ControllableMass;
        }

        private float ControllableMass => procAvionics.controllableMass;
>>>>>>> ProcAvionicsTooling
    }
}
