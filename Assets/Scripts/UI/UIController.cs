using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private GameObject StartPanel;
    [SerializeField]
    private GameObject DiedPanel;
    [SerializeField]
    private GameObject WinPanel;
    [SerializeField]
    private GameObject BossHealthPanel;

    private InputAction waitAnyKeyIA;

    private void Start()
    {
        // waitAnyKeyIA = new InputAction(binding: "<Keyboard>/<button>");
        // waitAnyKeyIA.performed += OnAnyKeyDown;

        TypeEventSystem.Global.Register<GameStartEvent>(OnStart).UnRegisterWhenGameObjectDestroyed(this);
        TypeEventSystem.Global.Register<PlayerDeathEvent>(OnDeath).UnRegisterWhenGameObjectDestroyed(this);
        TypeEventSystem.Global.Register<GameWinEvent>(OnWinGame).UnRegisterWhenGameObjectDestroyed(this);
        TypeEventSystem.Global.Register<GameLoseEvent>(OnLoseGame).UnRegisterWhenGameObjectDestroyed(this);

        Initialize();
        StartCoroutine(nameof(WaitForCheckKeyDown));
    }

    private IEnumerator WaitForCheckKeyDown()
    {
        yield return new WaitForSeconds(1.0f);
        // waitAnyKeyIA.Enable();
    }

    private void OnDestroy()
    {
        // waitAnyKeyIA.performed -= OnAnyKeyDown;
    }

    private async void OnAnyKeyDown(InputAction.CallbackContext context)
    {
        //Debug.Log(context.control.device.name);
        // waitAnyKeyIA.Disable();
        await Task.Yield();
        if (StartPanel.activeSelf)
        {
            TypeEventSystem.Global.Send<GameStartEvent>();
            StartPanel.SetActive(false);
        }

        if(DiedPanel.activeSelf)
        {
            // 视角变暗，重启游戏
            SceneManager.LoadScene(0);
        }

        if (WinPanel.activeSelf)
        {
            // 视角变暗，重启游戏
            SceneManager.LoadScene(0);
        }
    }

    private void Initialize()
    {
        StartPanel.SetActive(true);
        DiedPanel.SetActive(false);
        WinPanel.SetActive(false);
        BossHealthPanel.SetActive(false);
    }

    private void OnStart(GameStartEvent @event)
    {
    }

    private void OnDeath(PlayerDeathEvent @event)
    {
    }

    private void OnWinGame(GameWinEvent @event)
    {
        BossHealthPanel.SetActive(false);
        // waitAnyKeyIA.Enable();
        WinPanel.SetActive(true);
    }

    private void OnLoseGame(GameLoseEvent @event)
    {
        BossHealthPanel.SetActive(false);
        // waitAnyKeyIA.Enable();
        DiedPanel.SetActive(true);
    }

}
