using System;
using System.Collections.Generic;
using System.Globalization;
using GameCreator.Editor.Common.Versions;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(UpdatesRepository))]
    public class UpdatesRepositoryDrawer : PropertyDrawer
    {
        private static readonly TextInfo TXT = CultureInfo.InvariantCulture.TextInfo;
        private const string USS_PATH = EditorPaths.COMMON + "Settings/StyleSheets/Updates";

        private const string STORE_LINK = "https://gamecreator.link/{0}";

        private const string EXPAND_MORE = "+";
        private const string EXPAND_LESS = "-";
        
        private static readonly IIcon ICON_INSTALLED_YES = new IconCircleSolid(ColorTheme.Type.Green);
        private static readonly IIcon ICON_INSTALLED_NO = new IconCircleOutline(ColorTheme.Type.TextLight);

        private const string NAME_LOADING = "GC-Updates-Loading";
        
        private const string NAME_ASSET_ROOT = "GC-Updates-Asset-Root";
        private const string NAME_ASSET_HEAD = "GC-Updates-Asset-Head";
        private const string NAME_ASSET_BODY = "GC-Updates-Asset-Body";
        
        // MEMBERS: -------------------------------------------------------------------------------

        private VisualElement m_Content;

        // PAINT METHOD: --------------------------------------------------------------------------
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VersionsManager.Initialize();
            this.m_Content = new VisualElement();
            
            StyleSheet[] styleSheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet sheet in styleSheets) this.m_Content.styleSheets.Add(sheet);

            this.Refresh();
            VersionsManager.EventChange += this.Refresh;
            
            return this.m_Content;
        }

        private void Refresh()
        {
            this.m_Content.Clear();

            switch (VersionsManager.Latest.State)
            {
                case State.Loading: this.RefreshLoading(); break;
                case State.Ready: this.RefreshReady(); break;
                case State.Error: this.RefreshError(); break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void RefreshLoading()
        {
            Label loading = new Label("Fetching information...")
            {
                name = NAME_LOADING
            };
        
            this.m_Content.Add(loading);
        }
        
        private void RefreshError()
        {
            ErrorMessage error = new ErrorMessage("Error while fetching. Please check later");
            this.m_Content.Add(error);
        }
        
        private void RefreshReady()
        {
            foreach (KeyValuePair<string, AssetEntry> entry in VersionsManager.LatestEntries)
            {
                this.RefreshAsset(entry.Key, entry.Value);
            }
        }

        private void RefreshAsset(string id, AssetEntry asset)
        {
            VisualElement root = new VisualElement { name = NAME_ASSET_ROOT };
            VisualElement head = new VisualElement { name = NAME_ASSET_HEAD };
            VisualElement body = new VisualElement { name = NAME_ASSET_BODY };

            root.Add(head);
            root.Add(body);
            
            this.m_Content.Add(root);
            
            this.CreateHead(id, asset, head, body);
            this.CreateBody(id, asset, body);
        }

        private void CreateHead(string id, AssetEntry asset, VisualElement head, VisualElement body)
        {
            string path = RuntimePaths.PACKAGES + TXT.ToTitleCase(id);
            bool isInstalled = AssetDatabase.IsValidFolder(path); 
            
            Texture icon = isInstalled
                ? ICON_INSTALLED_YES.Texture
                : ICON_INSTALLED_NO.Texture;

            Button btnExpand = new Button
            {
                text = EXPAND_MORE,
                style =
                {
                    width = new Length(20, LengthUnit.Pixel),
                    borderRightWidth = new StyleFloat(1)
                }
            };
            
            btnExpand.clicked += () =>
            {
                body.style.display = body.style.display == DisplayStyle.None
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;

                btnExpand.text = body.style.display == DisplayStyle.None
                    ? EXPAND_MORE
                    : EXPAND_LESS;
            };

            Button btnInstall = new Button
            {
                text = isInstalled ? "Installed" : "Download",
                style =
                {
                    width = new Length(100, LengthUnit.Pixel),
                    borderLeftWidth = new StyleFloat(1)
                }
            };
            
            btnInstall.clicked += () =>
            {
                Application.OpenURL(string.Format(STORE_LINK, id));
            };
            
            btnInstall.SetEnabled(!isInstalled);

            head.Add(btnExpand);
            head.Add(new LabelTitle(TextUtils.Humanize(id)));
            head.Add(new Label(asset.Version.ToString()));
            head.Add(new Image { image = icon });
            head.Add(btnInstall);
        }

        private void CreateBody(string id, AssetEntry asset, VisualElement body)
        {
            body.Add(new LabelTitle($"Released on {asset.Release.Date}"));
            
            if (asset.Changes.New.Length > 0)
            {
                body.Add(new SpaceSmaller());
                body.Add(new LabelTitle("New"));
                foreach (string log in asset.Changes.New)
                {
                    body.Add(new Label($"- {log}"));
                }
            }
            
            if (asset.Changes.Enhanced.Length > 0)
            {
                body.Add(new SpaceSmaller());
                body.Add(new LabelTitle("Enhanced"));
                foreach (string log in asset.Changes.Enhanced)
                {
                    body.Add(new Label($"- {log}"));
                }
            }
            
            if (asset.Changes.Changed.Length > 0)
            {
                body.Add(new SpaceSmaller());
                body.Add(new LabelTitle("Changed"));
                foreach (string log in asset.Changes.Changed)
                {
                    body.Add(new Label($"- {log}"));
                }
            }
            
            if (asset.Changes.Removed.Length > 0)
            {
                body.Add(new SpaceSmaller());
                body.Add(new LabelTitle("Removed"));
                foreach (string log in asset.Changes.Removed)
                {
                    body.Add(new Label($"- {log}"));
                }
            }
            
            if (asset.Changes.Fixed.Length > 0)
            {
                body.Add(new SpaceSmaller());
                body.Add(new LabelTitle("Fixed"));
                foreach (string log in asset.Changes.Fixed)
                {
                    body.Add(new Label($"- {log}"));
                }
            }

            body.style.display = DisplayStyle.None;
        }
    }
}