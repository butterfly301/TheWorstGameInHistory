using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HotUpdate.Core;
using HotUpdate.Dialogue.Data;
using HotUpdate.Dialogue.Model;
using HotUpdate.Dialogue.System;
using HotUpdate.Dialogue.Utility;
using HotUpdate.Dialogue.View;
using HotUpdate.UI;
using QFramework;
using UnityEngine;

namespace HotUpdate.Dialogue.Controller
{
    /// <summary>
    ///     对话控制器
    ///     负责管理单个对话实例的流程、状态机、用户输入
    ///     可以创建多个实例，支持同时运行多个对话
    /// </summary>
    public class DialogueController : MonoBehaviour, IController
    {
        /// <summary>
        ///     对话完成回调
        /// </summary>
        public Action<string> OnDialogueComplete;

        /// <summary>
        ///     是否可以跳过打字
        /// </summary>
        private bool canSkipTyping = true;

        /// <summary>
        ///     当前对话数据
        /// </summary>
        private DialogueData currentDialogueData;

        /// <summary>
        ///     当前对话ID
        /// </summary>
        private string currentDialogueId;

        /// <summary>
        ///     当前节点ID
        /// </summary>
        private string currentNodeId;

        /// <summary>
        ///     当前对话状态
        /// </summary>
        private DialogueState currentState = DialogueState.Idle;

        /// <summary>
        ///     当前对话视图
        /// </summary>
        private IDialogueView currentView;

        /// <summary>
        ///     对话模型
        /// </summary>
        private DialogueModel dialogueModel;

        /// <summary>
        ///     对话系统
        /// </summary>
        private DialogueSystem dialogueSystem;

        /// <summary>
        ///     获取当前状态
        /// </summary>
        public DialogueState CurrentState => currentState;

        /// <summary>
        ///     获取当前对话ID
        /// </summary>
        public string CurrentDialogueId => currentDialogueId;

        private void Awake()
        {
            // 获取系统和模型
            dialogueSystem = this.GetSystem<DialogueSystem>();
            dialogueModel = this.GetModel<DialogueModel>();

            // 监听系统事件
            dialogueSystem.OnDialogueStart.AddListener(OnDialogueStartEvent);
            dialogueSystem.OnDialogueEnd.AddListener(OnDialogueEndEvent);
        }

        private void Update()
        {
            // 处理用户输入
            HandleInput();
        }

        private void OnDestroy()
        {
            // 取消监听系统事件
            if (dialogueSystem != null)
            {
                dialogueSystem.OnDialogueStart.RemoveListener(OnDialogueStartEvent);
                dialogueSystem.OnDialogueEnd.RemoveListener(OnDialogueEndEvent);
            }
        }

        public IArchitecture GetArchitecture()
        {
            return TheWorstGameInHistory.Interface;
        }

        /// <summary>
        ///     开始对话
        /// </summary>
        public void StartDialogue(string dialogueId)
        {
            if (currentState != DialogueState.Idle && currentState != DialogueState.Ended)
            {
                Debug.LogWarning($"[DialogueController] 无法开始对话，当前状态: {currentState}");
                return;
            }

            currentDialogueId = dialogueId;
            ChangeState(DialogueState.Loading);

            // 加载对话数据
            dialogueSystem.LoadDialogueDataAsync(
                dialogueId,
                data =>
                {
                    if (data == null)
                    {
                        Debug.LogError($"[DialogueController] 加载的数据为null!");
                        return;
                    }

                    currentDialogueData = data;

                    if (data.config == null)
                    {
                        Debug.LogError($"[DialogueController] data.config 为null!");
                        return;
                    }

                    canSkipTyping = data.config.canSkip;

                    // 检查是否需要恢复进度
                    if (data.config.saveProgress && dialogueModel.HasDialogueProgress(dialogueId))
                    {
                        var progress = dialogueModel.GetDialogueProgress(dialogueId);
                        currentNodeId = progress.currentNodeId;
                    }
                    else
                    {
                        currentNodeId = data.config.startNodeId;
                    }

                    // 创建视图
                    CreateDialogueView();

                    // 检查视图是否创建成功
                    if (currentView == null)
                    {
                        Debug.LogError($"[DialogueController] 视图创建失败，无法启动对话");
                        ChangeState(DialogueState.Idle);
                        currentDialogueId = null;
                        currentDialogueData = null;
                        OnDialogueComplete?.Invoke(dialogueId);
                        return;
                    }

                    // 开始对话
                    StartDialogueInternal();
                },
                error =>
                {
                    Debug.LogError($"[DialogueController] 加载对话数据失败: {error}");
                    ChangeState(DialogueState.Idle);
                    currentDialogueId = null;
                    currentDialogueData = null;

                    // 加载失败也触发完成回调
                    OnDialogueComplete?.Invoke(dialogueId);
                }
            );
        }

        /// <summary>
        ///     继续对话（按继续键）
        /// </summary>
        public void ContinueDialogue()
        {
            if (currentState == DialogueState.Typing)
            {
                // 如果正在打字，且允许跳过，则立即完成打字
                if (canSkipTyping) SkipTyping();
            }
            else if (currentState == DialogueState.Waiting)
            {
                // 进入下一个节点
                NextNode();
            }
            else
            {
                Debug.LogWarning($"[DialogueController] 无法继续对话，当前状态: {currentState}");
            }
        }

        /// <summary>
        ///     选择选项
        /// </summary>
        public void SelectChoice(int choiceIndex)
        {
            if (currentState != DialogueState.ShowingChoices)
            {
                Debug.LogWarning($"[DialogueController] 无法选择选项，当前状态: {currentState}");
                return;
            }

            var currentNode = currentDialogueData.config.nodes[currentNodeId];

            if (choiceIndex < 0 || choiceIndex >= currentNode.choices.Count)
            {
                Debug.LogError($"[DialogueController] 选项索引超出范围: {choiceIndex}");
                return;
            }

            var selectedChoice = currentNode.choices[choiceIndex];

            // 触发选项选择事件
            dialogueSystem.TriggerChoiceSelected(currentDialogueId, choiceIndex);

            // 跳转到下一个节点
            currentNodeId = selectedChoice.nextNodeId;
            ShowNode();
        }

        /// <summary>
        ///     跳过打字
        /// </summary>
        public void SkipTyping()
        {
            if (currentState == DialogueState.Typing && currentView != null)
            {
                currentView.CompleteTyping();
                ChangeState(DialogueState.Waiting);
            }
        }

        /// <summary>
        ///     结束对话
        /// </summary>
        public void EndDialogue()
        {
            if (currentState == DialogueState.Idle || currentState == DialogueState.Ended) return;

            // 保存进度
            if (currentDialogueData != null && currentDialogueData.config.saveProgress)
                dialogueModel.UpdateDialogueProgress(currentDialogueId, currentNodeId, true);

            // 隐藏视图
            if (currentView != null) currentView.Hide();

            // 触发对话结束事件
            dialogueSystem.TriggerDialogueEnd(currentDialogueId);

            // 清理数据
            var completedDialogueId = currentDialogueId;
            currentDialogueId = null;
            currentDialogueData = null;
            currentNodeId = null;
            currentView = null;

            ChangeState(DialogueState.Ended);

            // 触发完成回调
            OnDialogueComplete?.Invoke(completedDialogueId);

            // 延迟后返回空闲状态
            StartCoroutine(ReturnToIdleDelayed());
        }

        /// <summary>
        ///     暂停对话
        /// </summary>
        public void PauseDialogue()
        {
            if (currentState == DialogueState.Idle || currentState == DialogueState.Ended) return;

            ChangeState(DialogueState.Paused);
        }

        /// <summary>
        ///     恢复对话
        /// </summary>
        public void ResumeDialogue()
        {
            if (currentState != DialogueState.Paused) return;

            // 根据视图状态恢复
            if (currentView != null && currentView.IsTyping())
                ChangeState(DialogueState.Typing);
            else if (currentDialogueData != null && currentDialogueData.config.nodes[currentNodeId].choices != null &&
                     currentDialogueData.config.nodes[currentNodeId].choices.Count > 0)
                ChangeState(DialogueState.ShowingChoices);
            else
                ChangeState(DialogueState.Waiting);
        }

        /// <summary>
        ///     内部开始对话
        /// </summary>
        private void StartDialogueInternal()
        {
            // 触发对话开始事件
            dialogueSystem.TriggerDialogueStart(currentDialogueId);

            // 执行对话开始时的事件
            if (currentDialogueData.config.events?.onStart != null)
                foreach (var eventData in currentDialogueData.config.events.onStart)
                    dialogueSystem.ExecuteDialogueEvents(eventData, currentDialogueId);

            // 显示第一个节点
            ShowNode();
        }

        /// <summary>
        ///     显示节点
        /// </summary>
        private void ShowNode()
        {
            if (currentDialogueData == null || currentDialogueData.config == null || currentDialogueData.config.nodes == null)
            {
                Debug.LogError($"[DialogueController] 对话数据未正确加载");
                EndDialogue();
                return;
            }

            if (!currentDialogueData.config.nodes.ContainsKey(currentNodeId))
            {
                Debug.LogError($"[DialogueController] 节点不存在: {currentNodeId}");
                EndDialogue();
                return;
            }

            var node = currentDialogueData.config.nodes[currentNodeId];
            // 触发节点切换事件
            dialogueSystem.TriggerNodeChanged(currentDialogueId, currentNodeId);

            // 检查节点条件（条件分支）
            var conditionResult = DialogueCondition.CheckNodeConditions(node.conditions);

            if (!string.IsNullOrEmpty(conditionResult))
            {
                // 跳转到条件指定的节点
                currentNodeId = conditionResult;
                ShowNode();
                return;
            }

            // 执行节点事件（包括播放语音、自定义事件等）
            if (node.events != null)
                foreach (var eventData in node.events)
                    dialogueSystem.ExecuteDialogueEvents(eventData, currentDialogueId);

            // 获取本地化文本
            var localizedText = dialogueSystem.GetLocalizedText(node.textKey);

            // 显示对话
            currentView.ShowDialogue(node.speaker, localizedText);

            // 设置打字速度
            currentView.SetTypingSpeed(currentDialogueData.config.typingSpeed);

            // 检查是否有选项
            if (node.choices != null && node.choices.Count > 0)
                // 等待打字完成后显示选项
                StartCoroutine(ShowChoicesAfterTyping(node.choices));
            else
                // 没有选项，进入等待状态
                ChangeState(DialogueState.Typing);
        }

        /// <summary>
        ///     等待打字完成后显示选项
        /// </summary>
        private IEnumerator ShowChoicesAfterTyping(List<ChoiceData> choices)
        {
            ChangeState(DialogueState.Typing);

            // 等待打字完成
            while (currentView != null && currentView.IsTyping()) yield return null;

            // 过滤和显示选项
            var availableChoices = choices.Where(c =>
                DialogueCondition.CheckChoiceCondition(c.condition)
            ).ToArray();

            if (availableChoices.Length > 0)
            {
                currentView.ShowChoices(availableChoices, SelectChoice);
                ChangeState(DialogueState.ShowingChoices);
            }
            else
            {
                Debug.LogWarning("[DialogueController] 没有可用的选项");
                NextNode();
            }
        }

        /// <summary>
        ///     进入下一个节点
        /// </summary>
        private void NextNode()
        {
            var currentNode = currentDialogueData.config.nodes[currentNodeId];

            if (!string.IsNullOrEmpty(currentNode.nextNodeId))
            {
                // 有下一个节点
                currentNodeId = currentNode.nextNodeId;

                // 保存进度
                if (currentDialogueData.config.saveProgress)
                    dialogueModel.UpdateDialogueProgress(currentDialogueId, currentNodeId);

                ShowNode();
            }
            else
            {
                // 没有下一个节点，对话结束
                EndDialogue();
            }
        }

        /// <summary>
        ///     创建对话视图
        /// </summary>
        private void CreateDialogueView()
        {
            // 获取视图类型
            DialogueViewType viewType = currentDialogueData.config.viewType;

            // 通过 UIManager1 暴露的子组件入口获取并显示对话视图
            if (UIManager.Instance is UIManager1 uiManager1)
            {
                currentView = viewType switch
                {
                    DialogueViewType.Bubble => uiManager1.BubbleDialogue.GetDialogueView(),
                    DialogueViewType.Narrator => uiManager1.NarratorDialogue.GetDialogueView(),
                    DialogueViewType.Traditional => uiManager1.TraditionalDialogue.GetDialogueView(),
                    _ => null
                };
            }
            else
            {
                currentView = null;
            }
            if (currentView == null)
            {
                Debug.LogError($"[DialogueController] 无法获取对话视图，viewType: {viewType}");
                return;
            }
        }

        /// <summary>
        ///     处理用户输入
        /// </summary>
        private void HandleInput()
        {
            if (currentState == DialogueState.Idle || currentState == DialogueState.Ended ||
                currentState == DialogueState.Loading || currentState == DialogueState.Paused)
                return;

            // 检查继续键（可以根据项目需求修改按键）
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) ||
                Input.GetMouseButtonDown(0))
                ContinueDialogue();
        }

        /// <summary>
        ///     改变状态
        /// </summary>
        private void ChangeState(DialogueState newState)
        {
            if (currentState == newState) return;

            currentState = newState;
        }

        /// <summary>
        ///     延迟返回空闲状态
        /// </summary>
        private IEnumerator ReturnToIdleDelayed()
        {
            yield return new WaitForSeconds(0.5f);
            ChangeState(DialogueState.Idle);
        }

        /// <summary>
        ///     对话开始事件回调
        /// </summary>
        private void OnDialogueStartEvent(string dialogueId)
        {
        }

        /// <summary>
        ///     对话结束事件回调
        /// </summary>
        private void OnDialogueEndEvent(string dialogueId)
        {
        }
    }
}
