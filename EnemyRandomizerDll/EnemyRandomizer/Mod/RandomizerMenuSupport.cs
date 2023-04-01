using System.Collections;
using System.Collections.Generic;
using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using System.Linq;
using UnityEngine.UI;
using Modding.Menu;
using System;
using Modding.Menu.Config;
using static Modding.IMenuMod;
using UniRx;
using UniRx.Triggers;
using UniRx.Operators;
using UnityEngine.Events;

namespace EnemyRandomizerMod
{
    public partial class EnemyRandomizer
    {
        public static MenuBuilder CreateMenuBuilder(string Title)
        {
            return new MenuBuilder(UIManager.instance.UICanvas.gameObject, Title)
                .CreateTitle(Title, MenuTitleStyle.vanillaStyle)
                .CreateContentPane(RectTransformData.FromSizeAndPos(
                    new RelVector2(new Vector2(1920f, 903f)),
                    new AnchoredPosition(
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0f, -60f)
                    )
                ))
                .CreateControlPane(RectTransformData.FromSizeAndPos(
                    new RelVector2(new Vector2(1920f, 259f)),
                    new AnchoredPosition(
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0f, -502f)
                    )
                ))
                .SetDefaultNavGraph(new GridNavGraph(1));
        }

        public static void GoToMenuScreen(MenuScreen menuScreen) => UIManager.instance.UIGoToDynamicMenu(menuScreen);
    }

    public static class EnemyRandomizerMenuExtensions
    {
        public static MenuBuilder AddBackButton(this MenuBuilder builder, MenuScreen returnScreen, out UnityEngine.UI.MenuButton backButton)
        {
            UnityEngine.UI.MenuButton BackButton = null;
            builder.AddControls(
                new SingleContentLayout(new AnchoredPosition(
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0f, -64f)
                )), c => c.AddMenuButton(
                    "BackButton",
                    new MenuButtonConfig
                    {
                        Label = "BACK",
                        CancelAction = _ => EnemyRandomizer.GoToMenuScreen(returnScreen),
                        SubmitAction = _ => EnemyRandomizer.GoToMenuScreen(returnScreen),
                        Style = MenuButtonStyle.VanillaStyle,
                        Proceed = true
                    }, out BackButton));
            backButton = BackButton;
            return builder;
        }

        public static MenuBuilder AddBackButton(this MenuBuilder builder, MenuScreen returnScreen) => AddBackButton(builder, returnScreen, out _);

        public static MenuBuilder AddBackButton(this MenuBuilder builder, LogicMenu menuRef, out UnityEngine.UI.MenuButton backButton)
        {
            UnityEngine.UI.MenuButton BackButton = null;
            builder.AddControls(
                new SingleContentLayout(new AnchoredPosition(
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0f, -64f)
                )), c => c.AddMenuButton(
                    "BackButton",
                    new MenuButtonConfig
                    {
                        Label = "BACK",
                        CancelAction = _ => menuRef.CancelAction.Invoke(),
                        SubmitAction = _ => menuRef.CancelAction.Invoke(),
                        Style = MenuButtonStyle.VanillaStyle,
                        Proceed = true
                    }, out BackButton));
            backButton = BackButton;
            return builder;
        }
    }


    public abstract class BaseElement
    {
        public ReactiveProperty<string> Id { get; protected set; }
        public ReactiveProperty<string> Name { get; protected set; }
        public ReactiveProperty<GameObject> gameObject { get; protected set; }
        public ReactiveProperty<BaseElement> Parent { get; protected set; }
        public UnityEvent DoUpdate { get; protected set; }
        public UnityEvent<BaseElement> OnUpdate { get; protected set; }
        public ReactiveProperty<bool> IsVisible { get; protected set; }
        public UnityEvent<BaseElement> OnVisibleChanged { get; protected set; }

        protected CompositeDisposable disposables = new CompositeDisposable();

        protected virtual void Setup(string id, string name)
        {
            Id = new ReactiveProperty<string>();
            Name = new ReactiveProperty<string>();
            gameObject = new ReactiveProperty<GameObject>();
            Parent = new ReactiveProperty<BaseElement>();
            DoUpdate = new UnityEvent();
            OnUpdate = new UnityEvent<BaseElement>();
            IsVisible = new ReactiveProperty<bool>(true);
            OnVisibleChanged = new UnityEvent<BaseElement>();

            IsVisible.SkipLatestValueOnSubscribe()
                .Subscribe(_ =>
                {
                    DoUpdate.Invoke();
                    OnVisibleChanged.Invoke(this);
                }).AddTo(disposables);

            DoUpdate.AsObservable()
                .Subscribe(_ =>
                {
                    Update();
                    OnUpdate.Invoke(this);
                }).AddTo(disposables);

            OnUpdate.AsObservable()
                .Where(_ => Parent != null && Parent is IContainer)
                .Select(_ => Parent as IContainer)
                .Subscribe(x => x.Reflow()).AddTo(disposables);

            if (!string.IsNullOrEmpty(id))
                Id.Value = id;

            if (!string.IsNullOrEmpty(name))
                Name.Value = name;
        }

        public BaseElement()
        {
            Setup(null,null);
        }

        public BaseElement(string Id)
        {
            Setup(Id, null);
        }

        public BaseElement(string Id, string Name)
        {
            Setup(Id == "__UseName" ? Name : Id, Name);
        }

        public abstract void Update();
    }

    public abstract class Element : BaseElement
    {
        public Element()
            : base()
        {
        }

        public UnityEvent<Element> OnBuilt;

        public Element(string Id) : base(Id) { }
        public Element(string Id, string Name) : base(Id, Name) { }
        public abstract GameObjectRow Create(ContentArea c, LogicMenu Instance, bool AddToList = true);
    }

    public abstract class MenuElement : BaseElement
    {
        public MenuElement()
            :base()
        {
        }
    }

    public interface IContainer
    {
        Element Find(string Id);

        void Reflow(bool silent = false);

        UniRx.IObservable<BaseElement> OnReflow { get; }
        UniRx.IObservable<BaseElement> OnBuilt { get; }
    }

    public class GameObjectRow
    {

        /// <summary>
        /// The list of GameObjects that make up a Row.
        /// </summary>
        public List<GameObject> Row;

        /// <summary>
        /// The Parent Element (only not null for MenuRow)
        /// </summary>
        public Element Parent;

        /// <summary>
        /// Generates a GameObjectRow from a list of GameObjects.
        /// </summary>
        /// <param name="row">The list of GameObjects.</param>
        public GameObjectRow(List<GameObject> row)
        {
            this.Row = row;
        }

        /// <summary>
        /// Generates a GameObjectRow when there is only 1 GameObject
        /// </summary>
        /// <param name="FirstGo">The first GameObject to add.</param>
        public GameObjectRow(GameObject FirstGo)
        {
            this.Row = new List<GameObject> { FirstGo };
        }
        /// <summary>
        /// Generates a new GameObjectRow from the provided GameObjectRow.
        /// </summary>
        /// <param name="menuOptionGos">The GameObjectRow to build on.</param>
        public GameObjectRow(GameObjectRow menuOptionGos)
        {
            this.Row = menuOptionGos.Row;
        }
        /// <summary>
        /// Generates a new GameObjectRow from 2 GameObjectRows that contain a few GameObjects each
        /// </summary>
        /// <param name="firstRow">The first GameObjectRow.</param>
        /// <param name="secondRow">The second GameObjectRow.</param>
        public GameObjectRow(GameObjectRow firstRow, GameObjectRow secondRow)
        {
            this.Row = new List<GameObject>();
            firstRow.Row.ForEach(go => Row.Add(go));
            secondRow.Row.ForEach(go => Row.Add(go));
        }
        /// <summary>
        /// Generates an empty GameObjectRow. To be used as instead of null
        /// </summary>
        public GameObjectRow()
        {
            this.Row = new List<GameObject>();
        }

        /// <summary>
        /// a function to give the number of active elements in the GameObject row
        /// </summary>
        /// <returns>the number of active elements</returns>
        public int ActiveCount()
        {
            var count = 0;
            foreach (var go in Row)
            {
                if (go != null && go.activeInHierarchy)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
