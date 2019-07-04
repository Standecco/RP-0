using KSPAPIExtensions;
using RP0.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using static RP0.ProceduralAvionics.ProceduralAvionicsUtils;

namespace RP0.ProceduralAvionics
{
    class ModuleProceduralAvionics : ModuleAvionics, IPartMassModifier, IPartCostModifier
    {
        private const string KwFormat = "{0:0.##}";
        private const string WFormat = "{0:0}";
        private const float FloatTolerance = 1.00001f;
        private const float InternalTanksTotalVolumeUtilization = 0.246f; //Max utilization for 2 spheres within a cylindrical container worst case scenario
        private const float InternalTanksAvailableVolumeUtilization = 0.5f;

        #region Fields

<<<<<<< master
        [KSPField(isPersistant = true, guiName = "Contr. Mass", guiActiveEditor = true, guiUnits = "\u2009t", groupName = PAWGroup, groupDisplayName = PAWGroup),
         UI_FloatEdit(scene = UI_Scene.Editor, minValue = 0f, incrementLarge = 10f, incrementSmall = 1f, incrementSlide = 0.05f, sigFigs = 3, unit = "\u2009t")]
        public float controllableMass = -1;

        [KSPField(isPersistant = true, guiActiveEditor = true, guiName = "Configuration", groupName = PAWGroup, groupDisplayName = PAWGroup), UI_ChooseOption(scene = UI_Scene.Editor)]
        public string avionicsConfigName;

        [KSPField(isPersistant = true)]
        public string avionicsTechLevel;
=======
		const string kwFormat = "{0:0.##}";
		const string wFormat = "{0:0}";
        const float FLOAT_TOLERANCE = 1.00001f;

        [KSPField(isPersistant = true, guiName = "Contr. Mass", guiActive = false, guiActiveEditor = true, guiUnits = "\u2009t"),
		 UI_FloatEdit(scene = UI_Scene.Editor, minValue = 0f, incrementLarge = 10f, incrementSmall = 1f, incrementSlide = 0.05f, sigFigs = 3, unit = "\u2009t")]
		public float controllableMass = 0;

		[KSPField(isPersistant = true, guiActiveEditor = true, guiActive = false, guiName = "Configuration"), UI_ChooseOption(scene = UI_Scene.Editor)]
		public string avionicsConfigName;
		private string oldAvionicsConfigName;
>>>>>>> Start root avionics

        [KSPField(guiActiveEditor = true, guiName = "Avionics Utilization", groupName = PAWGroup)]
        public string utilizationDisplay;

<<<<<<< master
        [KSPField(guiActiveEditor = true, guiName = "Power Requirements", groupName = PAWGroup)]
        public string powerRequirementsDisplay;

        [KSPField(guiActiveEditor = true, guiName = "Avionics Mass", groupName = PAWGroup)]
        public string massDisplay;

        [KSPField(guiActiveEditor = true, guiName = "Avionics Cost", groupName = PAWGroup)]
        public string costDisplay;

        public bool IsScienceCore => CurrentProceduralAvionicsTechNode.massExponent == 0 && CurrentProceduralAvionicsTechNode.powerExponent == 0 && CurrentProceduralAvionicsTechNode.costExponent == 0;

        [KSPField(guiName = "Desired Utilization", guiActiveEditor = true, guiFormat = "P1", groupName = PAWGroup),
         UI_FloatRange(scene = UI_Scene.Editor, minValue = .01f, maxValue = .999f, stepIncrement = .001f, suppressEditorShipModified = true)]
        public float targetUtilization = 1;

        private static bool _configsLoaded = false;

        private bool _started = false;

        [KSPEvent(active = true, guiActiveEditor = true, guiName = "Resize to Utilization", groupName = PAWGroup)]
        void SeekVolume()
        {
            if (GetSeekVolumeMethod(out PartModule PPart) is System.Reflection.MethodInfo method)
            {
                float targetVolume = GetAvionicsVolume() / targetUtilization;
                Log($"SeekVolume() target utilization {targetUtilization:P1}, CurrentAvionicsVolume for max util: {GetAvionicsVolume()}, Desired Volume: {targetVolume}");
                try
                {
                    method.Invoke(PPart, new object[] { targetVolume });
                }
                catch (Exception e) { Debug.LogError($"{e?.InnerException.Message ?? e.Message}"); }
            }
        }
=======
        [KSPField(isPersistant = true)]
        public float avionicsDensity;

        [KSPField(isPersistant = true)]
        public float massExponent;

        [KSPField(isPersistant = true)]
        public float massConstant;

        [KSPField(isPersistant = true)]
        public float massFactor;

        [KSPField(isPersistant = true)]
        public float shieldingMassFactor;

        [KSPField(isPersistant = true)]
        public float costExponent;

        [KSPField(isPersistant = true)]
        public float costConstant;

        [KSPField(isPersistant = true)]
        public float costFactor;

        [KSPField(isPersistant = true)]
        public float powerExponent;

        [KSPField(isPersistant = true)]
        public float powerConstant;

        [KSPField(isPersistant = true)]
        public float powerFactor;

        [KSPField(isPersistant = true)]
        public float disabledPowerFactor;

        [KSPField(isPersistant = true)]
		public int SASServiceLevel = -1;
>>>>>>> Start root avionics

        private System.Reflection.MethodInfo GetSeekVolumeMethod(out PartModule PPart)
        {
            PPart = null;
            if (part.Modules.Contains("ProceduralPart"))
            {
                PPart = part.Modules["ProceduralPart"];
                System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;
                return PPart.GetType().GetMethod("SeekVolume", flags);
            }
            return null;
        }

<<<<<<< master
        public ProceduralAvionicsConfig CurrentProceduralAvionicsConfig { get; private set; }
=======
		[KSPField(isPersistant = false, guiActive = false, guiActiveEditor = true, guiName = "Avionics Utilization")]
		public string utilizationDisplay;
>>>>>>> Use minimum value > 0 for controllable mass

        public ProceduralAvionicsTechNode CurrentProceduralAvionicsTechNode
        {
            get
            {
                if (CurrentProceduralAvionicsConfig != null && avionicsTechLevel != null && CurrentProceduralAvionicsConfig.TechNodes.ContainsKey(avionicsTechLevel))
                {
                    return CurrentProceduralAvionicsConfig.TechNodes[avionicsTechLevel];
                }
                return new ProceduralAvionicsTechNode();
            }
        }

<<<<<<< master
        public float Utilization => GetAvionicsMass() / MaxAvionicsMass;

        private float MaxAvionicsMass => cachedVolume * CurrentProceduralAvionicsTechNode.avionicsDensity;
=======
		[KSPField(isPersistant = false, guiActive = false, guiActiveEditor = true, guiName = "Avionics Mass")]
		public string massDisplay;

		[KSPField(isPersistant = false, guiActive = false, guiActiveEditor = true, guiName = "Avionics Cost")]
		public string costDisplay;
>>>>>>> Use a real fuels tank for avionics

<<<<<<< master
        public float InternalTanksVolume { get; private set; }
=======
		public ProceduralAvionicsConfig CurrentProceduralAvionicsConfig {
			get { return currentProceduralAvionicsConfig; }
		}
>>>>>>> Start root avionics

<<<<<<< master
        #endregion

<<<<<<< master
        #region Get Utilities
        protected override float GetInternalMassLimit() => !IsScienceCore ? controllableMass : 0;
=======
        public ProceduralAvionicsTechNode CurrentProceduralAvionicsTechNode {
			get {
				if (CurrentProceduralAvionicsConfig != null && avionicsTechLevel != null && CurrentProceduralAvionicsConfig.TechNodes.ContainsKey(avionicsTechLevel))
				{
					return CurrentProceduralAvionicsConfig.TechNodes[avionicsTechLevel];
				}
				return null;
			}
		}
>>>>>>> Use a real fuels tank for avionics

        private void ClampControllableMass()
        {
            var maxControllableMass = GetMaximumControllableMass();
            if (controllableMass > maxControllableMass * FloatTolerance)
=======
		protected override float GetInternalMassLimit()
		{
<<<<<<< master
            var oldLimit = proceduralMassLimit;
            ClampInternalMassLimit();
            if(proceduralMassLimit != oldLimit)
>>>>>>> Start root avionics
=======
            var oldLimit = controllableMass;
            ClampControllableMass();
            if(controllableMass != oldLimit)
>>>>>>> Renamings
            {
                Log($"Resetting procedural mass limit to {maxControllableMass}, was {controllableMass}");
                controllableMass = maxControllableMass;
                MonoUtilities.RefreshContextWindows(part);
            }
<<<<<<< master
        }

        private float GetControllableMass(float avionicsMass) 
        {
            float res = GetInversePolynomial(avionicsMass * 1000, CurrentProceduralAvionicsTechNode.massExponent, CurrentProceduralAvionicsTechNode.massConstant, CurrentProceduralAvionicsTechNode.massFactor);
            return float.IsNaN(res) || float.IsInfinity(res) ? 0 : res;
        }
//        private float GetMaximumControllableMass() => FloorToSliderIncrement(GetControllableMass(MaxAvionicsMass));
        private float GetMaximumControllableMass() => GetControllableMass(MaxAvionicsMass);

        private float GetAvionicsMass() => GetPolynomial(GetInternalMassLimit(), CurrentProceduralAvionicsTechNode.massExponent, CurrentProceduralAvionicsTechNode.massConstant, CurrentProceduralAvionicsTechNode.massFactor) / 1000f;
        private float GetAvionicsCost() => GetPolynomial(GetInternalMassLimit(), CurrentProceduralAvionicsTechNode.costExponent, CurrentProceduralAvionicsTechNode.costConstant, CurrentProceduralAvionicsTechNode.costFactor);
        private float GetAvionicsVolume() => GetAvionicsMass() / CurrentProceduralAvionicsTechNode.avionicsDensity;

        private float GetShieldedAvionicsMass()
=======
            return controllableMass;
		}

        private void ClampControllableMass()
>>>>>>> Renamings
        {
            var avionicsMass = GetAvionicsMass();
            return avionicsMass + GetShieldingMass(avionicsMass);
        }

        private float GetShieldingMass(float avionicsMass) => Mathf.Pow(avionicsMass, 2f / 3) * CurrentProceduralAvionicsTechNode.shieldingMassFactor;

        protected override float GetEnabledkW() => GetPolynomial(GetInternalMassLimit(), CurrentProceduralAvionicsTechNode.powerExponent, CurrentProceduralAvionicsTechNode.powerConstant, CurrentProceduralAvionicsTechNode.powerFactor) / 1000f;
        protected override float GetDisabledkW() => GetEnabledkW() * CurrentProceduralAvionicsTechNode.disabledPowerFactor;

        private static float GetPolynomial(float value, float exponent, float constant, float factor) => (Mathf.Pow(value, exponent) + constant) * factor;
        private static float GetInversePolynomial(float value, float exponent, float constant, float factor) => Mathf.Pow(value / factor - constant, 1 / exponent);

        protected override bool GetToggleable() => CurrentProceduralAvionicsTechNode.disabledPowerFactor > 0;

        protected override string GetTonnageString() => "This part can be configured to allow control of vessels up to any mass.";

        public float GetModuleMass(float defaultMass, ModifierStagingSituation sit) => CurrentProceduralAvionicsTechNode.avionicsDensity > 0 ? GetShieldedAvionicsMass() : 0;
        public ModifierChangeWhen GetModuleMassChangeWhen() => ModifierChangeWhen.FIXED;
        public float GetModuleCost(float defaultCost, ModifierStagingSituation sit) => CurrentProceduralAvionicsTechNode.avionicsDensity > 0 ? GetAvionicsCost() : 0;
        public ModifierChangeWhen GetModuleCostChangeWhen() => ModifierChangeWhen.FIXED;

        #endregion

        #region Callbacks

        public override void OnLoad(ConfigNode node)
        {
<<<<<<< master
            if (HighLogic.LoadedScene == GameScenes.LOADING && !_configsLoaded)
=======
            var max = GetMaximumControllableMass();
            if (max == 0)
>>>>>>> Use minimum value > 0 for controllable mass
            {
<<<<<<< master
<<<<<<< master
<<<<<<< master
                try
                {
                    Log("Loading Avionics Configs");
                    ProceduralAvionicsTechManager.LoadAvionicsConfigs();
                    _configsLoaded = true;
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
=======
                Log("NO MAX");
=======
                Log("WARNING: NO MAX");
>>>>>>> Add shielding mass
                return;
=======
                Log($"WARNING: NO MAX volume: {cachedVolume} MaxAvMass: {MaxAvionicsMass} max controllable mass: {GetControllableMass(MaxAvionicsMass)}");
>>>>>>> ProcAvionicsTooling
            }

            if (controllableMass > max * FLOAT_TOLERANCE)
            {
                Log("Resetting procedural mass limit to max of ", max, ", was ", controllableMass);
                controllableMass = max;
                RefreshPartWindow();
>>>>>>> Start root avionics
            }

<<<<<<< master
<<<<<<< master
            base.OnLoad(node);
        }
=======
=======
        private float GetMaximumControllableMass() => FloorToSliderIncrement(GetControllableMass(MaxAvionicsMass));

<<<<<<< master
>>>>>>> Remove Reset to 100 Functionality
        private float GetControllableMass(float avionicsMass)
        {
            var mass = GetInversePolynomial(avionicsMass * 1000, massExponent, massConstant, massFactor);
            Log($"Controllable mass: {mass}, avionicsMass: {avionicsMass}, Exp: {massExponent}, C: {massConstant}, Fac: {massFactor}");
            return mass;
        }
=======
        private float GetControllableMass(float avionicsMass) => GetInversePolynomial(avionicsMass * 1000, massExponent, massConstant, massFactor);
>>>>>>> ProcAvionicsTooling

        private float GetShieldedAvionicsMass()
        {
            var avionicsMass = GetAvionicsMass();
            return avionicsMass + GetShieldingMass(avionicsMass);
        }

        private float GetAvionicsMass() => GetPolynomial(GetInternalMassLimit(), massExponent, massConstant, massFactor) / 1000f;

        private float GetShieldingMass(float avionicsMass) => Mathf.Pow(avionicsMass, 2f / 3) * shieldingMassFactor;

        private float GetAvionicsCost() => GetPolynomial(GetInternalMassLimit(), costExponent, costConstant, costFactor);

        protected override float GetEnabledkW() => GetPolynomial(GetInternalMassLimit(), powerExponent, powerConstant, powerFactor) / 1000f;

        private static float GetPolynomial(float value, float exponent, float constant, float factor) => (Mathf.Pow(value, exponent) + constant) * factor;

        private static float GetInversePolynomial(float value, float exponent, float constant, float factor) => Mathf.Pow(value / factor - constant, 1 / exponent);

        protected override float GetDisabledkW() => GetEnabledkW() * disabledPowerFactor;

        protected override bool GetToggleable() => disabledPowerFactor > 0;

        protected override string GetTonnageString() => "This part can be configured to allow control of vessels up to any mass.";

        private ProceduralAvionicsConfig currentProceduralAvionicsConfig;
		private UI_FloatEdit controllableMassEdit;

		#endregion

		#region event handlers
		public override void OnLoad(ConfigNode node)
		{
			try {
				Log("OnLoad called");
				if (GameSceneFilter.Loading.IsLoaded()) {
					Log("Loading Avionics Configs");
					ProceduralAvionicsTechManager.LoadAvionicsConfigs(node);
				}
			}
			catch (Exception ex) {
				Log("OnLoad exception: ", ex.Message);
				throw;
			}
		}

		private bool started = false;
		public new void Start()
		{
			Log("Start called");

			string config = ProceduralAvionicsTechManager.GetPurchasedConfigs()[0];
			Log("Default config to use: ", config);

			if (String.IsNullOrEmpty(avionicsTechLevel)) {
				avionicsTechLevel = ProceduralAvionicsTechManager.GetMaxUnlockedTech(
					String.IsNullOrEmpty(avionicsConfigName) ? config : avionicsConfigName);
				Log("No tech level set, using ", avionicsTechLevel);
			}

			if (String.IsNullOrEmpty(avionicsConfigName)) {
				Log("No config set, using ", config);
				avionicsConfigName = config;
			}

			UpdateConfigSliders();
			BindUIChangeCallbacks();
>>>>>>> Start root avionics

        public override void OnStart(StartState state)
        {
            Log($"OnStart for {part} in {HighLogic.LoadedScene}");
            SetFallbackConfigForLegacyCraft();
            SetupConfigNameFields();
            SetControllableMassForLegacyCraft();
            AvionicsConfigChanged();
            SetupGUI();
            base.OnStart(state);
            massLimit = controllableMass;
            _started = true;
            if (cachedEventData != null)
                OnPartVolumeChanged(cachedEventData);
        }

<<<<<<< master
        public void Start()
        {
            // Delay SetScienceContainer to Unity.Start() to allow PartModule removal
            if (!HighLogic.LoadedSceneIsEditor)
                SetScienceContainer();
        }
=======
			if (cachedEventData != null) {
				OnPartVolumeChanged(cachedEventData);
			}

			base.Start();
            started = true;
            Log("Start finished");
		}

		private bool callbacksBound = false;
		private void BindUIChangeCallbacks()
		{
			if (!callbacksBound) {
				Fields[nameof(controllableMass)].uiControlEditor.onFieldChanged += ControllableMassChanged;
				Fields[nameof(avionicsConfigName)].uiControlEditor.onFieldChanged += AvionicsConfigChanged;
				callbacksBound = true;
			}
		}

		private void ControllableMassChanged(BaseField arg1, object arg2)
		{
			Log("Mass limit changed");
            ClampControllableMass();
            SetMinVolume();
            SendRemainingVolume();
            RefreshDisplays();
		}
>>>>>>> Start root avionics

        #endregion

        #region OnStart Utilities

        private void SetFallbackConfigForLegacyCraft()
        {
            if (HighLogic.LoadedSceneIsEditor && !ProceduralAvionicsTechManager.GetAvailableConfigs().Contains(avionicsConfigName))
            {
                string s = avionicsConfigName;
                avionicsConfigName = ProceduralAvionicsTechManager.GetPurchasedConfigs().First();
                Log($"Current config ({s}) not available, defaulting to {avionicsConfigName}");
            }
<<<<<<< master
            if (string.IsNullOrEmpty(avionicsTechLevel))
=======
            Log("Setting config to ", avionicsConfigName);
            currentProceduralAvionicsConfig = ProceduralAvionicsTechManager.GetProceduralAvionicsConfig(avionicsConfigName);
            Log("Setting tech node to ", avionicsTechLevel);
            oldAvionicsConfigName = avionicsConfigName;
            oldAvionicsTechLevel = avionicsTechLevel;
            SetInternalKSPFields();
            ClampControllableMass();
            SetMinVolume(true);
            UpdateControllableMassSlider();
            SendRemainingVolume();
            OnConfigurationUpdated();
            RefreshDisplays();
		}


		private float cachedMinVolume = float.MaxValue;
		public void SetMinVolume(bool forceUpdate = false)
		{
			Log("Setting min volume for proceduralMassLimit of ", controllableMass);
			float minVolume = GetAvionicsMass() / avionicsDensity * FLOAT_TOLERANCE;
			if (float.IsNaN(minVolume)) {
				return;
			}
			Log("min volume should be ", minVolume);
			cachedMinVolume = minVolume;

			PartModule ppModule = null;
			Type ppModuleType = null;
			foreach (var module in part.Modules) {
				var moduleType = module.GetType();
				if (moduleType.FullName == "ProceduralParts.ProceduralPart") {
					ppModule = module;
					ppModuleType = moduleType;
					ppModuleType.GetField("volumeMin").SetValue(ppModule, minVolume);
					Log("Applied min volume");
				}
			}
			Log("minVolume: ", minVolume);
			Log("Comparing against cached volume of ", cachedVolume);
			if (forceUpdate || minVolume > cachedVolume) {
				if (!forceUpdate) {
					Log("cachedVolume too low: ", cachedVolume);
				}
				if (ppModule != null) {
					var reflectedShape = ppModuleType.GetProperty("CurrentShape").GetValue(ppModule, null);
					reflectedShape.GetType().GetMethod("ForceNextUpdate").Invoke(reflectedShape, new object[] { });
					Log("Volume fixed, refreshing part window");
				}
				RefreshPartWindow();
			}
		}

        public void FixedUpdate()
		{
			if (!HighLogic.LoadedSceneIsEditor) {
				SetSASServiceLevel();
				SetScienceContainer();
			}
		}

        public float GetModuleMass(float defaultMass, ModifierStagingSituation sit) => GetMassSafely();
        public ModifierChangeWhen GetModuleMassChangeWhen() => ModifierChangeWhen.FIXED;
        public float GetModuleCost(float defaultCost, ModifierStagingSituation sit) => GetCostSafely();
        public ModifierChangeWhen GetModuleCostChangeWhen() => ModifierChangeWhen.FIXED;

        #endregion


        #region part attribute calculations
        private float GetMassSafely()
		{
            return avionicsDensity > 0 ? GetShieldedAvionicsMass() : 0;
        }

		private float GetCostSafely()
		{
            return avionicsDensity > 0 ? GetAvionicsCost() : 0;
        }

		#endregion

<<<<<<< master
		private float GetMaximumControllableMass()
		{
            return FloorToSliderIncrement(GetControllableMass(MaxAvionicsMass));
		}

		private void ResetTo100()
		{
            if(cachedVolume == float.MaxValue)
>>>>>>> Start root avionics
            {
                avionicsTechLevel = ProceduralAvionicsTechManager.GetMaxUnlockedTech(avionicsConfigName);
                Log($"Defaulting avionics tech level to {avionicsTechLevel}");
            }
<<<<<<< master
<<<<<<< master
<<<<<<< master
        }
=======
            proceduralMassLimit = GetControllableMass(MaxAvionicsMass);
=======
            controllableMass = GetControllableMass(MaxAvionicsMass);
>>>>>>> Renamings
=======
            controllableMass = GetMaximumControllableMass();
>>>>>>> Use minimum value > 0 for controllable mass
		}
>>>>>>> Start root avionics

<<<<<<< master
        private void SetControllableMassForLegacyCraft()
        {
            if (controllableMass < 0)
            {
                controllableMass = HighLogic.LoadedSceneIsFlight ? float.MaxValue : 0;
            }
        }

<<<<<<< master
<<<<<<< master
<<<<<<< master
        private void SetupConfigNameFields()
        {
            Fields[nameof(avionicsConfigName)].guiActiveEditor = true;
            var range = Fields[nameof(avionicsConfigName)].uiControlEditor as UI_ChooseOption;
            range.options = ProceduralAvionicsTechManager.GetPurchasedConfigs().ToArray();
=======
			if (proceduralMassLimitEdit == null) {
				proceduralMassLimitEdit = (UI_FloatEdit)Fields[nameof(proceduralMassLimit)].uiControlEditor;
=======
=======
		private void UpdateMaxValues()
=======
=======
>>>>>>> Remove Reset to 100 Functionality
		private void UpdateControllableMassSlider()
>>>>>>> Use minimum value > 0 for controllable mass
		{
>>>>>>> Use a real fuels tank for avionics
			if (controllableMassEdit == null) {
				controllableMassEdit = (UI_FloatEdit)Fields[nameof(controllableMass)].uiControlEditor;
>>>>>>> Renamings
			}
>>>>>>> Start root avionics

            if (string.IsNullOrEmpty(avionicsConfigName))
            {
<<<<<<< master
<<<<<<< master
                avionicsConfigName = range.options[0];
                Log($"Defaulted config to {avionicsConfigName}");
            }
        }

        private void SetupGUI()
        {
            Fields[nameof(controllableMass)].uiControlEditor.onFieldChanged = ControllableMassChanged;
            Fields[nameof(avionicsConfigName)].uiControlEditor.onFieldChanged = AvionicsConfigChanged;
            Fields[nameof(massLimit)].guiActiveEditor = false;
            if (!(GetSeekVolumeMethod(out PartModule _) is System.Reflection.MethodInfo))
            {
<<<<<<< master
                Events[nameof(SeekVolume)].active = Events[nameof(SeekVolume)].guiActiveEditor = false;
                Fields[nameof(targetUtilization)].guiActiveEditor = false;
            }
        }

        #endregion

        private void UpdateControllableMassSlider()
        {
            Fields[nameof(controllableMass)].guiActiveEditor = !IsScienceCore;
            UI_FloatEdit controllableMassEdit = Fields[nameof(controllableMass)].uiControlEditor as UI_FloatEdit;

            if (CurrentProceduralAvionicsConfig != null && CurrentProceduralAvionicsTechNode != null)
=======
                proceduralMassLimitEdit.maxValue = CeilingToSmallIncrement(GetMaximumControllableTonnage());
                proceduralMassLimitEdit.minValue = 0;
=======
                controllableMassEdit.maxValue = CeilingToSmallIncrement(GetMaximumControllableTonnage());
                controllableMassEdit.minValue = 0;
>>>>>>> Renamings
=======
                controllableMassEdit.maxValue = CeilingToSmallIncrement(GetMaximumControllableMass());
<<<<<<< master
>>>>>>> Use minimum value > 0 for controllable mass
=======
                controllableMassEdit.minValue = 0;
>>>>>>> ProcAvionicsTooling

                controllableMassEdit.incrementSmall = GetSmallIncrement(controllableMassEdit.maxValue);
                controllableMassEdit.incrementLarge = controllableMassEdit.incrementSmall * 10;
                controllableMassEdit.incrementSlide = GetSliderIncrement(controllableMassEdit.maxValue);
                controllableMassEdit.sigFigs = GetSigFigs(controllableMassEdit.maxValue);
            }
            else
>>>>>>> Start root avionics
            {
<<<<<<< master
<<<<<<< master
                // Formula for controllable mass given avionics mass is Mathf.Pow(1000*avionicsMass / massFactor - massConstant, 1 / massExponent)
                controllableMassEdit.maxValue = Mathf.Max(GetMaximumControllableMass(), 0.001f);
=======
                Log("Cannot update max value yet, CurrentProceduralAvionicsConfig is null");
<<<<<<< master
                controllableMassEdit.maxValue = float.MaxValue;
                controllableMassEdit.minValue = 0;
>>>>>>> Renamings
=======
>>>>>>> Use minimum value > 0 for controllable mass
=======
                Log("WARNING: Cannot update max value yet, CurrentProceduralAvionicsConfig is null");
>>>>>>> ProcAvionicsTooling
            }
            else
                controllableMassEdit.maxValue = 0.001f;
            Log($"UpdateControllableMassSlider() MaxCtrlMass: {controllableMassEdit.maxValue}");
            controllableMassEdit.minValue = 0;
            controllableMassEdit.incrementSmall = GetSmallIncrement(controllableMassEdit.maxValue);
            controllableMassEdit.incrementLarge = controllableMassEdit.incrementSmall * 10;
            controllableMassEdit.incrementSlide = GetSliderIncrement(controllableMassEdit.maxValue);
            controllableMassEdit.sigFigs = GetSigFigs(controllableMassEdit.maxValue);
            controllableMassEdit.maxValue = FloorToPrecision(controllableMassEdit.maxValue, controllableMassEdit.incrementSlide);
        }

        #region UI Slider Tools

        private int GetSigFigs(float value)
        {
            var smallIncrementExponent = GetSmallIncrementExponent(value);
            return Math.Max(1 - (int)smallIncrementExponent, 0);
        }

        private float CeilingToSmallIncrement(float value)
        {
            var smallIncrement = GetSmallIncrement(value);
            return Mathf.Ceil(value / smallIncrement) * smallIncrement;
        }

<<<<<<< master
        private float FloorToPrecision(float value, float precision) => Mathf.Floor(value / precision) * precision;
=======
        private float FloorToSliderIncrement(float value)
        {
            float sliderIncrement = GetSliderIncrement(value);
            return Mathf.Floor(value / sliderIncrement) * sliderIncrement;
        }
>>>>>>> Start root avionics

        private float GetSliderIncrement(float value)
        {
            var smallIncrement = GetSmallIncrement(value);
            return Math.Min(smallIncrement / 10, 1f);
        }

        private float GetSmallIncrement(float value)
        {
            var exponent = GetSmallIncrementExponent(value);
            return (float)Math.Pow(10, exponent);
        }

        private double GetSmallIncrementExponent(float maxValue)
        {
            var log = Math.Log(maxValue, 10);
            return Math.Max(Math.Floor(log - 1.3), -2);
        }

<<<<<<< master
        #endregion

        private float cachedVolume = float.MaxValue;
        private BaseEventDetails cachedEventData = null;

        #region Events and Change Handlers

        private void ControllableMassChanged(BaseField arg1, object arg2)
        {
            Log($"ControllableMassChanged to {arg1.GetValue(this)} from {arg2}");
            if (float.IsNaN(controllableMass))
            {
                Debug.LogError("ProcAvi - ControllableMassChanged tried to set to NAN! Resetting to 0.");
                controllableMass = 0;
=======
        private void UpdateConfigSliders()
		{
			Log("Updating Config Slider");
			BaseField avionicsConfigField = Fields[nameof(avionicsConfigName)];
			avionicsConfigField.guiActiveEditor = true;
			UI_ChooseOption range = (UI_ChooseOption)avionicsConfigField.uiControlEditor;
			range.options = ProceduralAvionicsTechManager.GetPurchasedConfigs().ToArray();

			if (String.IsNullOrEmpty(avionicsConfigName)) {
				avionicsConfigName = range.options[0];
				Log("Defaulted config to ", avionicsConfigName);
			}
		}

		private float cachedVolume = float.MaxValue;
		private BaseEventDetails cachedEventData = null;

		[KSPEvent]
		public void OnPartVolumeChanged(BaseEventDetails eventData)
		{
			Log("OnPartVolumeChanged called");
			if (!started) {
				Log("Not yet started, returning");
				cachedEventData = eventData;
				return;
			}
			try
            {
                float volume = (float)eventData.Get<double>("newTotalVolume");
                Log("volume changed to ", volume);
                if (volume * FLOAT_TOLERANCE < cachedMinVolume && cachedMinVolume != float.MaxValue)
                {
                    Log("volume of ", volume, " is less than expected min volume of ", cachedMinVolume, " expecting another update");
                    RefreshPartWindow();
                    //assuming the part will be resized
                    return;
                }
                Log("setting cachedVolume to ", volume);
                cachedVolume = volume;
                SendRemainingVolume();
                UpdateControllableMassSlider();
                RefreshDisplays();
>>>>>>> Start root avionics
            }
<<<<<<< master
            ClampControllableMass();
            massLimit = controllableMass;
            SendRemainingVolume();
            RefreshDisplays();
        }

        private void AvionicsConfigChanged(BaseField f, object obj)
        {
            avionicsTechLevel = ProceduralAvionicsTechManager.GetMaxUnlockedTech(avionicsConfigName);
            AvionicsConfigChanged();
        }

<<<<<<< master
        private void AvionicsConfigChanged()
=======
            catch (Exception ex) {
				Log("error getting changed volume: ", ex);
			}
		}

        private void SendRemainingVolume()
        {
            if(cachedVolume == float.MaxValue)
            {
                return;
            }
            Log($"Sending remaining volume: {cachedVolume - GetAvionicsMass() / avionicsDensity}");
            Events[nameof(OnPartVolumeChanged)].active = false;
            SendVolumeChangedEvent(cachedVolume - GetAvionicsMass() / avionicsDensity);
            Events[nameof(OnPartVolumeChanged)].active = true;
        }

        public void SendVolumeChangedEvent(double newVolume)
        {
            var data = new BaseEventDetails(BaseEventDetails.Sender.USER);
            data.Set<string>("volName", "Tankage");
            data.Set<double>("newTotalVolume", newVolume);
            part.SendEvent(nameof(OnPartVolumeChanged), data, 0);
        }

        private void SetInternalKSPFields()
>>>>>>> Propagate spare volume
        {
            CurrentProceduralAvionicsConfig = ProceduralAvionicsTechManager.GetProceduralAvionicsConfig(avionicsConfigName);
            Log($"Avionics Config changed to: {avionicsConfigName}.  Tech: {avionicsTechLevel}");
            interplanetary = CurrentProceduralAvionicsTechNode.interplanetary;
            if (_started)
            {
                // Don't fire these if cachedVolume isn't known yet.
                UpdateMassLimitsAndVolume();
            }
            if (!GetToggleable())
                systemEnabled = true;
            SetActionsAndGui();
            RefreshDisplays();
            if (HighLogic.LoadedSceneIsEditor)
                GameEvents.onEditorShipModified.Fire(EditorLogic.fetch.ship);
        }

        [KSPEvent]
        public void OnPartVolumeChanged(BaseEventDetails eventData)
        {
            float volume = (float)eventData.Get<double>("newTotalVolume");
            Log($"OnPartVolumeChanged to {volume} from {cachedVolume}");
            if (!_started)
            {
                Log("Delaying OnPartVolumeChanged until after Start()");
                cachedEventData = eventData;
                return;
            }
            cachedVolume = volume;
            UpdateMassLimitsAndVolume();
            RefreshDisplays();
        }
=======
            massExponent = CurrentProceduralAvionicsTechNode.massExponent;
            massConstant = CurrentProceduralAvionicsTechNode.massConstant;
            massFactor = CurrentProceduralAvionicsTechNode.massFactor;
            shieldingMassFactor = CurrentProceduralAvionicsTechNode.shieldingMassFactor;
            costExponent = CurrentProceduralAvionicsTechNode.costExponent;
            costConstant = CurrentProceduralAvionicsTechNode.costConstant;
            costFactor = CurrentProceduralAvionicsTechNode.costFactor;
            powerExponent = CurrentProceduralAvionicsTechNode.powerExponent;
            powerConstant = CurrentProceduralAvionicsTechNode.powerConstant;
            powerFactor = CurrentProceduralAvionicsTechNode.powerFactor;
            disabledPowerFactor = CurrentProceduralAvionicsTechNode.disabledPowerFactor;
            avionicsDensity = CurrentProceduralAvionicsTechNode.avionicsDensity;
>>>>>>> Start root avionics

        private void UpdateMassLimitsAndVolume()
        {
            ClampControllableMass();
            UpdateControllableMassSlider();
            SendRemainingVolume();
        }

        private void SendRemainingVolume()
        {
            if (_started && cachedVolume < float.MaxValue)
            {
                Events[nameof(OnPartVolumeChanged)].active = false;
                InternalTanksVolume = SphericalTankUtilities.GetSphericalTankVolume(GetAvailableVolume());
                float availVol = GetAvailableVolume();
                Log($"SendRemainingVolume():  Cached Volume: {cachedVolume}. AvionicsVolume: {GetAvionicsVolume()}.  AvailableVolume: {availVol}.  Internal Tanks: {InternalTanksVolume}");
                SendVolumeChangedEvent(InternalTanksVolume);
                Events[nameof(OnPartVolumeChanged)].active = true;
            }
        }

        private float GetAvailableVolume() => Math.Max(Math.Min((cachedVolume - GetAvionicsVolume()) * InternalTanksAvailableVolumeUtilization, cachedVolume * InternalTanksTotalVolumeUtilization), 0);

        public void SendVolumeChangedEvent(double newVolume)
        {
            var data = new BaseEventDetails(BaseEventDetails.Sender.USER);
            data.Set<string>("volName", "Tankage");
            data.Set<double>("newTotalVolume", newVolume);
            part.SendEvent(nameof(OnPartVolumeChanged), data, 0);
        }

        #endregion

<<<<<<< master
        public new static string BackgroundUpdate(Vessel v,
            ProtoPartSnapshot part_snapshot, ProtoPartModuleSnapshot module_snapshot,
            PartModule proto_part_module, Part proto_part,
            Dictionary<string, double> availableResources, List<KeyValuePair<string, double>> resourceChangeRequest,
            double elapsed_s) => ModuleAvionics.BackgroundUpdate(v, part_snapshot, module_snapshot, proto_part_module, proto_part, availableResources, resourceChangeRequest, elapsed_s);
=======
            utilizationDisplay = String.Format("{0:0.#}%", Utilization* 100);
            Log("Utilization display: ", utilizationDisplay);
>>>>>>> Make dimension tooling cost based on utilization

        private void RefreshDisplays()
        {
            RefreshPowerDisplay();
            massDisplay = MathUtils.FormatMass(CurrentProceduralAvionicsTechNode.avionicsDensity > 0 ? GetShieldedAvionicsMass() : 0);
            costDisplay = $"{Mathf.Round(CurrentProceduralAvionicsTechNode.avionicsDensity > 0 ? GetAvionicsCost() : 0)}";
            utilizationDisplay = $"{Utilization * 100:0.#}%";
            Log($"RefreshDisplays() Controllable mass: {controllableMass}, mass: {massDisplay} cost: {costDisplay}, Utilization: {utilizationDisplay}");
        }

        public float Utilization => GetAvionicsMass() / MaxAvionicsMass;

        private float MaxAvionicsMass => cachedVolume * avionicsDensity;

        private void RefreshPowerDisplay()
        {
<<<<<<< master
            var powerConsumptionBuilder = StringBuilderCache.Acquire();
            AppendPowerString(powerConsumptionBuilder, GetEnabledkW());
            float dkW = GetDisabledkW();
            if (dkW > 0)
=======
            StringBuilder powerConsumptionBuilder = StringBuilderCache.Acquire();
            double kW = GetEnabledkW();
<<<<<<< master
            if (kW >= 0.1)
>>>>>>> Start root avionics
=======
            if (kW >= 1)
>>>>>>> ProcAvionicsTooling
            {
                powerConsumptionBuilder.Append(" /");
                AppendPowerString(powerConsumptionBuilder, dkW);
            }
            powerRequirementsDisplay = powerConsumptionBuilder.ToStringAndRelease();
        }

        private void AppendPowerString(System.Text.StringBuilder builder, float val)
        {
            if (val >= 1)
                builder.AppendFormat(KwFormat, val).Append("\u2009kW");
            else
                builder.AppendFormat(WFormat, val * 1000).Append("\u2009W");
        }

        private void SetScienceContainer()
        {
            if (!CurrentProceduralAvionicsTechNode.hasScienceContainer)
            {
                if (part.FindModuleImplementing<ModuleScienceContainer>() is ModuleScienceContainer module)
                    part.RemoveModule(module);
            }
            Log($"Setting science container to {(CurrentProceduralAvionicsTechNode.hasScienceContainer ? "enabled." : "disabled.")}");
        }

        [KSPField(guiActiveEditor = true, guiName = "Configure", groupName = PAWGroup),
        UI_Toggle(enabledText = "Hide GUI", disabledText = "Show GUI"),
        NonSerialized]
        public bool showGUI;

        private Rect windowRect = new Rect(200, Screen.height - 400, 400, 300);

        public void OnGUI()
        {
            if (showGUI)
            {
                windowRect = GUILayout.Window(GetInstanceID(), windowRect, WindowFunction, "Configure Procedural Avionics");
            }
        }

        private int selectedConfigIndex = 0;
        void WindowFunction(int windowID)
        {
            var configNames = ProceduralAvionicsTechManager.GetAvailableConfigs().ToArray();
            selectedConfigIndex = GUILayout.Toolbar(selectedConfigIndex, configNames);
            var guiAvionicsConfigName = configNames[selectedConfigIndex];
            var currentlyDisplayedConfigs = ProceduralAvionicsTechManager.GetProceduralAvionicsConfig(guiAvionicsConfigName);
            foreach (var techNode in currentlyDisplayedConfigs.TechNodes.Values)
            {
                if (!techNode.IsAvailable)
                {
                    continue;
                }
                if (techNode == CurrentProceduralAvionicsTechNode)
                {
                    GUILayout.Label("Current Config: " + techNode.name);
                    GUILayout.Label("Storage Container: " + (techNode.hasScienceContainer ? "Yes" : "No"));
                }
                else
                {
                    var switchedConfig = false;
                    var unlockCost = ProceduralAvionicsTechManager.GetUnlockCost(guiAvionicsConfigName, techNode);
                    if (unlockCost == 0)
                    {
                        if (GUILayout.Button("Switch to " + BuildTechName(techNode)))
                        {
                            switchedConfig = true;
                        }
                    }
                    else if (Funding.Instance.Funds < unlockCost)
                    {
                        GUILayout.Label("Can't afford " + BuildTechName(techNode) + BuildCostString(unlockCost));
                    }
                    else if (GUILayout.Button("Purchase " + BuildTechName(techNode) + BuildCostString(unlockCost)))
                    {
                        switchedConfig = true;
                        if (!HighLogic.CurrentGame.Parameters.Difficulty.BypassEntryPurchaseAfterResearch)
                        {
                            switchedConfig = ProceduralAvionicsTechManager.PurchaseConfig(guiAvionicsConfigName, techNode);
                        }
                        if (switchedConfig)
                        {
                            ProceduralAvionicsTechManager.SetMaxUnlockedTech(guiAvionicsConfigName, techNode.name);
                        }

                    }
                    if (switchedConfig)
                    {
                        Log("Configuration window changed, updating part window");
                        SetupConfigNameFields();
                        avionicsTechLevel = techNode.name;
                        CurrentProceduralAvionicsConfig = currentlyDisplayedConfigs;
                        avionicsConfigName = guiAvionicsConfigName;
                        AvionicsConfigChanged();
                        MonoUtilities.RefreshContextWindows(part);
                    }
                }
            }
            GUILayout.Label(" ");
            if (GUILayout.Button("Close"))
            {
                showGUI = false;
            }

            GUI.DragWindow();
        }

<<<<<<< master
<<<<<<< master
        private string BuildTechName(ProceduralAvionicsTechNode techNode)
        {
            var sbuilder = StringBuilderCache.Acquire();
            sbuilder.Append(techNode.name);
            sbuilder.Append(BuildSasAndScienceString(techNode));

            return sbuilder.ToStringAndRelease();
        }

        private static string BuildSasAndScienceString(ProceduralAvionicsTechNode techNode) => techNode.hasScienceContainer ? " {SC}" : "";

        private string BuildCostString(int cost) =>
            (cost == 0 || HighLogic.CurrentGame.Parameters.Difficulty.BypassEntryPurchaseAfterResearch) ? string.Empty : $" ({cost:N})";
    }
=======
        // creating a field for this so we don't need to look it up every update
=======
>>>>>>> Use minimum value > 0 for controllable mass
        private ModuleSAS sasModule = null;
		private void SetSASServiceLevel()
		{
			if (SASServiceLevel >= 0) {
				if (sasModule == null) {
					sasModule = part.FindModuleImplementing<ModuleSAS>();
				}
				if (sasModule != null) {
					if (sasModule.SASServiceLevel != SASServiceLevel) {
						sasModule.SASServiceLevel = SASServiceLevel;
						Log("Setting SAS service level to ", SASServiceLevel);
					}
				}
			}
		}

		private bool scienceContainerFiltered = false;
		private void SetScienceContainer()
		{
			if (scienceContainerFiltered) {
				return;
			}
			if (!hasScienceContainer) {
				var module = part.FindModuleImplementing<ModuleScienceContainer>();
				if (module != null) {
					part.RemoveModule(module);
				}
			}
			Log("Setting science container to ", (hasScienceContainer ? "enabled." : "disabled."));
			scienceContainerFiltered = true;
		}

        private void RefreshCostAndMassDisplays()
		{
			massDisplay = MathUtils.FormatMass(GetMassSafely());
			costDisplay = Mathf.Round(GetCostSafely()).ToString();
		}

        #region Config GUI
        [KSPField(isPersistant = false, guiActiveEditor = true, guiActive = false, guiName = "Configure"),
		UI_Toggle(enabledText = "Hide GUI", disabledText = "Show GUI"),
		NonSerialized]
		public bool showGUI;

		private Rect windowRect = new Rect(200, Screen.height - 400, 400, 300);

		public void OnGUI()
		{
			if (showGUI) {
				windowRect = GUILayout.Window(GetInstanceID(), windowRect, WindowFunction, "Configure Procedural Avionics");
			}
		}

		private int selectedConfigIndex = 0;
		void WindowFunction(int windowID)
		{
			string[] configNames = ProceduralAvionicsTechManager.GetAvailableConfigs().ToArray();

			selectedConfigIndex = GUILayout.Toolbar(
				selectedConfigIndex,
				configNames);

			string guiAvionicsConfigName = configNames[selectedConfigIndex];

			ProceduralAvionicsConfig currentlyDisplayedConfigs =
				ProceduralAvionicsTechManager.GetProceduralAvionicsConfig(guiAvionicsConfigName);
			foreach (ProceduralAvionicsTechNode techNode in currentlyDisplayedConfigs.TechNodes.Values) {
				if (!techNode.IsAvailable) {
					continue;
				}
				if (techNode == CurrentProceduralAvionicsTechNode) {
					GUILayout.Label("Current Config: " + techNode.name);
					GUILayout.Label("SAS Level: " + techNode.SASServiceLevel.ToString());
					GUILayout.Label("Storage Container: " + (techNode.hasScienceContainer ? "Yes" : "No"));
				}
				else {
					bool switchedConfig = false;
					int unlockCost = ProceduralAvionicsTechManager.GetUnlockCost(guiAvionicsConfigName, techNode);
					if (unlockCost == 0) {
						if (GUILayout.Button("Switch to " + BuildTechName(techNode))) {
							switchedConfig = true;
						}
					}
					else if (Funding.Instance.Funds < unlockCost) {
						GUILayout.Label("Can't afford " + BuildTechName(techNode) + BuildCostString(unlockCost));
					}
					else if (GUILayout.Button("Purchase " + BuildTechName(techNode) + BuildCostString(unlockCost))) {
						switchedConfig = true;
						if (!HighLogic.CurrentGame.Parameters.Difficulty.BypassEntryPurchaseAfterResearch) {
							switchedConfig = ProceduralAvionicsTechManager.PurchaseConfig(guiAvionicsConfigName, techNode);
						}
						if (switchedConfig) {
							ProceduralAvionicsTechManager.SetMaxUnlockedTech(guiAvionicsConfigName, techNode.name);
						}

					}
					if (switchedConfig) {
						Log("Configuration window changed, updating part window");
						UpdateConfigSliders();
						avionicsTechLevel = techNode.name;
						currentProceduralAvionicsConfig = currentlyDisplayedConfigs;
						avionicsConfigName = guiAvionicsConfigName;
						AvionicsConfigChanged();
                    }
				}
			}
			GUILayout.Label(" ");
			if (GUILayout.Button("Close")) {
				showGUI = false;
			}

			GUI.DragWindow();
		}

		private string BuildTechName(ProceduralAvionicsTechNode techNode)
		{
			StringBuilder sbuilder = StringBuilderCache.Acquire();
			sbuilder.Append(techNode.name);
			sbuilder.Append(BuildSasAndScienceString(techNode));

			return sbuilder.ToStringAndRelease();
		}

		private static string BuildSasAndScienceString(ProceduralAvionicsTechNode techNode)
		{
			StringBuilder sbuilder = StringBuilderCache.Acquire();
			sbuilder.Append(" {SAS: ");
			sbuilder.Append(techNode.SASServiceLevel);
			if (techNode.hasScienceContainer) {
				sbuilder.Append(", SC");
			}
			sbuilder.Append("}");

			return sbuilder.ToString();
		}

		private string BuildCostString(int cost)
		{
			if (cost == 0 || HighLogic.CurrentGame.Parameters.Difficulty.BypassEntryPurchaseAfterResearch) {
				return String.Empty;
			}
			return " (" + String.Format("{0:N}", cost) + ")";
		}

		#endregion

		private void RefreshPartWindow()
		{
			UIPartActionWindow[] partWins = FindObjectsOfType<UIPartActionWindow>();
			foreach (UIPartActionWindow partWin in partWins) {
				partWin.displayDirty = true;
			}
		}
	}
>>>>>>> Start root avionics
}
