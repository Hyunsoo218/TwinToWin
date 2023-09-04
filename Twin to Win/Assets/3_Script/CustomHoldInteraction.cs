using UnityEngine;
using UnityEngine.InputSystem;

//!!>> This script should NOT be placed in an "Editor" folder. Ideally placed in a "Plugins" folder.
namespace Invertex.UnityInputExtensions.Interactions
{
    //https://gist.github.com/Invertex
    /// <summary>
    /// Custom Hold interaction for New Input System.
    /// With this, the .performed callback will be called everytime the Input System updates. 
    /// Allowing a purely callback based approach to a button hold instead of polling it in an Update() loop or creating specific logic for it
    /// .started will be called when the 'pressPoint' threshold has been met and held for the 'duration' (unless 'Trigger .started on Press Point' is checked).
    /// .performed will continue to be called each frame after `.started` has triggered (or every amount of time set for "Performed Interval")
    /// .cancelled will be called when no-longer actuated (but only if the input has actually 'started' triggering
    /// </summary>
#if UNITY_EDITOR
    using UnityEditor;
    //Allow for the interaction to be utilized outside of Play Mode and so that it will actually show up as an option in the Input Manager
    [UnityEditor.InitializeOnLoad]
#endif
    [UnityEngine.Scripting.Preserve, System.ComponentModel.DisplayName("Holding"), System.Serializable]
    public class CustomHoldingInteraction : IInputInteraction
    {
        public float delayBetweenPerformed = 0f;
        public bool triggerStartedOnPressPoint = false;

        public bool useDefaultSettingsPressPoint = true;
        public float pressPoint = InputSystem.settings.defaultButtonPressPoint;

        public bool useDefaultSettingsDuration = true;
        public float duration = InputSystem.settings.defaultHoldTime;

        private float _heldTime = 0f;

        private float pressPointOrDefault => useDefaultSettingsPressPoint || pressPoint <= 0 ? InputSystem.settings.defaultButtonPressPoint : pressPoint;
        private float durationOrDefault => useDefaultSettingsDuration || duration < 0 ? InputSystem.settings.defaultHoldTime : duration;

        private InputInteractionContext ctx;

        private void OnUpdate()
        {
            var isActuated = ctx.ControlIsActuated(pressPointOrDefault);
            var phase = ctx.phase;

            //Cancel and cleanup our action if it's no-longer actuated or been externally changed to a stopped state.
            if (phase == InputActionPhase.Canceled || phase == InputActionPhase.Disabled || !ctx.action.actionMap.enabled || !isActuated)
            {
                Cancel(ref ctx);
                return;
            }

            _heldTime += Time.deltaTime;

            bool holdDurationElapsed = _heldTime >= durationOrDefault;

            if (!holdDurationElapsed && !triggerStartedOnPressPoint) { return; }
            if (phase == InputActionPhase.Waiting) { ctx.Started(); return; }
            if (!holdDurationElapsed) { return; }
            if (phase == InputActionPhase.Started) { ctx.PerformedAndStayPerformed(); return; }

            float heldMinusDelay = _heldTime - delayBetweenPerformed;

            //Held time has exceed our minimum hold time, plus any delay we've set,
            //so perform it and assign back the hold time without the delay time to let increment back up again
            if (heldMinusDelay >= durationOrDefault)
            {
                _heldTime = heldMinusDelay;
                ctx.PerformedAndStayPerformed();
            }
        }

        public void Process(ref InputInteractionContext context)
        {
            ctx = context; //Ensure our Update always has access to the most recently updated context

            if (!ctx.ControlIsActuated(pressPointOrDefault)) { Cancel(ref context); return; } //Actuation changed and thus no longer performed, cancel it all.

            if (ctx.phase != InputActionPhase.Performed && ctx.phase != InputActionPhase.Started)
            {
                EnableInputHooks();
            }
        }

        private void Cleanup()
        {
            DisableInputHooks();
            _heldTime = 0f;
        }

        private void Cancel(ref InputInteractionContext context)
        {
            Cleanup();

            if (context.phase == InputActionPhase.Performed || context.phase == InputActionPhase.Started)
            { //Input was being held when this call was made. Trigger the .cancelled event.
                context.Canceled();
            }
        }
        public void Reset() => Cleanup();

        private void OnLayoutChange(string layoutName, InputControlLayoutChange change) => Reset();
        private void OnDeviceChange(InputDevice device, InputDeviceChange change) => Reset();
#if UNITY_EDITOR
        private void PlayModeStateChange(UnityEditor.PlayModeStateChange state) => Reset();
#endif
        private void EnableInputHooks()
        {
            InputSystem.onAfterUpdate -= OnUpdate; //Safeguard for duplicate registrations
            InputSystem.onAfterUpdate += OnUpdate;
            //In case layout or device changes, we'll want to trigger a cancelling of the current input action subscription to avoid errors.
            InputSystem.onLayoutChange -= OnLayoutChange;
            InputSystem.onLayoutChange += OnLayoutChange;
            InputSystem.onDeviceChange -= OnDeviceChange;
            InputSystem.onDeviceChange += OnDeviceChange;
            //Prevent the update hook from persisting across a play mode change to avoid errors.

#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged -= PlayModeStateChange;
            UnityEditor.EditorApplication.playModeStateChanged += PlayModeStateChange;
#endif
        }


        private void DisableInputHooks()
        {
            InputSystem.onAfterUpdate -= OnUpdate;
            InputSystem.onLayoutChange -= OnLayoutChange;
            InputSystem.onDeviceChange -= OnDeviceChange;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged -= PlayModeStateChange;
#endif
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void RegisterInteraction()
        {
            if (InputSystem.TryGetInteraction("CustomHolding") == null)
            { //For some reason if this is called again when it already exists, it permanently removees it from the drop-down options... So have to check first
                InputSystem.RegisterInteraction<CustomHoldingInteraction>("CustomHolding");
            }
        }

        //Constructor will be called by our Editor [InitializeOnLoad] attribute when outside Play Mode
        static CustomHoldingInteraction() => RegisterInteraction();
    }

#if UNITY_EDITOR
    internal class CustomHoldInteractionEditor : UnityEngine.InputSystem.Editor.InputParameterEditor<CustomHoldingInteraction>
    {
        private static GUIContent pressPointWarning, holdTimeWarning, pressPointLabel, holdTimeLabel, startedTriggerOnPPLabel, startedTriggerOnPPToggleLabel, delayPerformedLabel;

        protected override void OnEnable()
        {
            delayPerformedLabel = new GUIContent("'Performed' interval (s)", $"Delay in seconds between each <b>.performed</b> call.{System.Environment.NewLine}" +
            "At the default value of 0, it will be every frame.");

            startedTriggerOnPPLabel = new GUIContent("Trigger 'Started' on Press Point", $"Trigger the <b>.started</b> event as soon as input actuated beyond \"Press Point\",{System.Environment.NewLine}" +
                $"instead of waiting for the \"Min Hold Time\" as well.");
            startedTriggerOnPPToggleLabel = new GUIContent("", startedTriggerOnPPLabel.tooltip);

            pressPointLabel = new GUIContent("Press Point", $"The minimum amount this input's actuation value must exceed to be considered \"held\".{System.Environment.NewLine}" +
            "Value less-than or equal to 0 will result in the 'Default Button Press Point' value being used from your 'Project Settings > Input System'.");

            holdTimeLabel = new GUIContent("Min Hold Time", $"The minimum amount of realtime seconds before the input is considered \"held\".{System.Environment.NewLine}" +
            "Value less-than or equal to 0 will result in the 'Default Hold Time' value being used from your 'Project Settings > Input System'.");

            pressPointWarning = EditorGUIUtility.TrTextContent("Using \"Default Button Press Point\" set in project-wide input settings.");
            holdTimeWarning = EditorGUIUtility.TrTextContent("Using \"Default Hold Time\" set in project-wide input settings.");
        }

        public override void OnGUI()
        {
            target.delayBetweenPerformed = EditorGUILayout.FloatField(delayPerformedLabel, target.delayBetweenPerformed, GUILayout.ExpandWidth(false));
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(startedTriggerOnPPLabel, GUILayout.Width(205f));
            target.triggerStartedOnPressPoint = GUILayout.Toggle(target.triggerStartedOnPressPoint, startedTriggerOnPPToggleLabel, GUILayout.ExpandWidth(false));
            EditorGUILayout.EndHorizontal();

            DrawDisableIfDefault(ref target.pressPoint, ref target.useDefaultSettingsPressPoint, pressPointLabel, pressPointWarning);
            DrawDisableIfDefault(ref target.duration, ref target.useDefaultSettingsDuration, holdTimeLabel, holdTimeWarning, -Mathf.Epsilon);
        }

        private void DrawDisableIfDefault(ref float value, ref bool useDefault, GUIContent fieldName, GUIContent warningText, float compareOffset = 0f)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(useDefault);
            value = EditorGUILayout.FloatField(fieldName, value, GUILayout.ExpandWidth(false));
            value = Mathf.Clamp(value, 0f, float.MaxValue);
            EditorGUI.EndDisabledGroup();

            GUIContent content =
                EditorGUIUtility.TrTextContent("Default",
                                               $"If enabled, the default {fieldName.text.ToLower()} " +
                                               $"configured globally in the input settings is used.{System.Environment.NewLine}" +
                                               "See Edit >> Project Settings... >> Input System Package.");
            useDefault = GUILayout.Toggle(useDefault, content, GUILayout.ExpandWidth(false));
            EditorGUILayout.EndHorizontal();

            if (useDefault || value <= 0 + compareOffset)
            {
                EditorGUILayout.HelpBox(warningText);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUIContent settingsLabel = EditorGUIUtility.TrTextContent("Open Input Settings");
                if (GUILayout.Button(settingsLabel, EditorStyles.miniButton))
                    SettingsService.OpenProjectSettings("Project/Input System Package");
                EditorGUILayout.EndHorizontal();
            }
        }
    }
#endif
}