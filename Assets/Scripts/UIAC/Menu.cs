using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    CanvasGroup Group;

    UIController Controller;

    InputMaster Controls;

    [HideInInspector]
    public bool Opened;

    public float FadeTime = 0.25f;

    public bool PauseGame;

    public bool hideHud;

    public bool CustomControlMethod;

    float Target;
    float Actual;

    void Update() {
        if (Actual == Target)
            return;

        Actual = Mathf.MoveTowards(Actual, Target, UnityEngine.Time.unscaledDeltaTime);

        Group.alpha = 1 * DynamicUtil.EvaluateCurve(DynamicUtil.EvalCurve.Sine, Actual / FadeTime);
    }

    public void Open() {
        Target = FadeTime;
        Group.blocksRaycasts = true;
        Opened = true;

        if (PauseGame) {
            Time.timeScale = 0;
            // Controller.Player.controls.Disable();
        }

        if (CustomControlMethod) {
            // Controller.Controls.Disable();
            Controls.Enable();
        }
    }

    public void Close() {
        Target = 0;
        Group.blocksRaycasts = false;
        Opened = false;

        if (PauseGame) {
            Time.timeScale = 1;
            // Controller.Player.controls.Enable();
        }

        if (CustomControlMethod) {
            // Controller.Controls.Enable();
            // Controller.Player.controls.Disable();
        }
    }

    public virtual void Exit() {
        MenuController.CloseMenus();
    }

    void Start()
    {
        Group = GetComponent<CanvasGroup>();
        Controller = FindFirstObjectByType<UIController>();
        if (CustomControlMethod)
            Controls = new InputMaster();
    }
}
