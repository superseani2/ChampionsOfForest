﻿using ChampionsOfForest.Player;
using System;
using System.Collections;
using TheForest.Utils;
using UnityEngine;
using ResourceLoader = ChampionsOfForest.Res.ResourceLoader;

namespace ChampionsOfForest
{
    public class MainMenu : MonoBehaviour
    {
        //Instance of this object
        public static MainMenu Instance { get; private set; }

        //Difficulty Settings
        public string[] DiffSel_Names = new string[] { "Normal", "Hard", "Elite", "Master", "Challenge I", "Challenge II", "Challenge III", "Challenge IV", "Challenge V", };
        public string[] DiffSel_Descriptions = new string[]
        {
            "Difficulty for beginners \n Enemy stats at 100% of their base value \n Exp rate - 100% \n Loot rarity - 100%",
            "Difficulty for somewhat intermediate players \n Enemy stats at 300% of their base value \n Exp rate - 200% \n Loot rarity - 200%",
            "Difficulty for levels 30-50 \n Enemy stats at 1 200% of their base value \n Exp rate - 900% \n Loot rarity - 900%",
            "Difficulty for levels 50+ with good equipement \n Enemy stats at 15 000% of their base value \n Exp rate - 10000% \n Loot rarity - 10 000%",
            "Challenge I-V unlock best tier item drops \n Enemy stats at 45 000% of their base value \n Exp rate - 40 000% \n Loot rarity - 40 000%",
            "Challenge I-V unlock best tier item drops \n Enemy stats at 100 000% of their base value \n Exp rate - 85 000% \n Loot rarity - 85 000%",
            "Challenge I-V unlock best tier item drops \n Enemy stats at 250 000% of their base value \n Exp rate - 40 000% \n Loot rarity - 40 000%",
            "Challenge I-V unlock best tier item drops \n Enemy stats at 1 000 000% of their base value \n Exp rate - 500 000% \n Loot rarity - 500 000%",
            "Challenge I-V unlock best tier item drops \n Enemy stats at 10 000 000% of their base value \n Exp rate - 1 000 000% \n Loot rarity - 1 000 000%",

        };
        private int DiffSelPage = 0;

        //variables
        public float rr;                                // current resolution to fullHD ratio
        private Vector2 mousepos;                       //mouse postion in GUI space
        private Rect wholeScreen;                       //rect that contains the entire screen
        private bool Crouching;                         //used in hud display, when crouching scans enemies
        private bool MenuInteractable;                  //determines if menu elements can be clicked or not
        private GUIStyle MenuBtnStyle;                  //style of font for main menu button
        private readonly float DarkeningSpeed = 2;      //speed of transion between menus
        public Font MainFont;                           //main font for the mod
        private float requestResendTime = 0;            //float that measures time between difficulty request times


        //main menu vars
        //alphas for button highlight image
        private float Opacity1 = 0;
        private float Opacity2 = 0;
        private float Opacity3 = 0;
        private float Opacity4 = 0;
        private readonly float OpacityGainSpeed = 3;    //speed at which the highlights gain alpha - curently gains full opacity after 0.33 seconds


        //HUD variables
        private float ScanTime = 0;
        private float ScanRotation = 0;
        private Transform scannedTransform;
        private BoltEntity scannedEntity;
        private bool HideHud = false;

        private Rect HUDenergyLabelRect;
        private Rect HUDHealthLabelRect;

        private GUIStyle HUDStatStyle;

        //Inventory variables
        private float InventoryScrollAmount = 0;
        private int SelectedItem;
        public int DraggedItemIndex;

        public bool isDragging;

        public Item DraggedItem = null;
        private Vector2 itemPos;
        private Vector2 slotDim;


        //Textures
        private Texture2D _black;
        private Texture2D _blackTexture;
        private Texture2D _combatDurationTex;
        private Texture2D _expBarFillTex;
        private Texture2D _expBarBackgroundTex;
        private Texture2D _NoSpellIcon;
        private Texture2D _SpellBG;
        private Texture2D _SpellInactive;
        private Texture2D _SpellFrame;
        private Texture2D _SpellCoolDownFill;
        private Texture2D _expBarFrameTex;





        //a static variable for colors of items with different rarities
        //affects item border in inventory, text color in pickup, particle effect color 
        public static Color[] RarityColors = new Color[]
        {
            new Color(0.4f,0.4f,0.4f),
            new Color(0.6f,0.6f,0.6f),
            new Color(0.1f,0.1f,0.9f),
            new Color(0.1f,0.1f,0.9f),
            new Color(1,0.95f,0.1f),
            new Color(1,0.5f,0f),
            new Color(0.74f,0.05f,0.05f),
            new Color(0.1f,1,0.3f)
        };

        public enum OpenedMenuMode
        {
            Main,
            Inventory,
            Hud,
            Skills,
            Stats,
            Perks,
        }
        public OpenedMenuMode _openedMenu;

        //setting the instance
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {

            try
            {
                rr = Screen.height / 1080f;
                wholeScreen = new Rect(0, 0, Screen.width, Screen.height);

                slotDim = new Vector2(100 * rr, 100 * rr);

                _black = new Texture2D(1, 1);
                _black.SetPixel(0, 0, Color.black);
                _black.Apply();
                _openedMenu = OpenedMenuMode.Hud;
                _blackTexture = new Texture2D(1, 1);
                _blackTexture.SetPixel(0, 0, new Color(0, 0, 0, 0));
                _blackTexture.Apply();

                MenuInteractable = true;


                //HUD
                HideHud = false;
                HUDenergyLabelRect = new Rect(Screen.width - 500 * rr, Screen.height - 100 * rr, 500 * rr, 100 * rr);
                HUDHealthLabelRect = new Rect(Screen.width - 500 * rr, Screen.height - 140 * rr, 500 * rr, 100 * rr);

                //The main font as of now is Gabriola
                MainFont = Font.CreateDynamicFontFromOSFont("Gabriola", Mathf.RoundToInt(24 * rr));

                //Getting textures using ResourceLoader
                _combatDurationTex = ResourceLoader.instance.LoadedTextures[18];
                _expBarFillTex = ResourceLoader.instance.LoadedTextures[16];
                _expBarBackgroundTex = ResourceLoader.instance.LoadedTextures[15];
                _expBarFrameTex = ResourceLoader.instance.LoadedTextures[17];
                _NoSpellIcon = ResourceLoader.instance.LoadedTextures[5];
                _SpellBG = ResourceLoader.instance.LoadedTextures[5];
                _SpellInactive = ResourceLoader.instance.LoadedTextures[5];
                _SpellFrame = ResourceLoader.instance.LoadedTextures[6];
                _SpellCoolDownFill = ResourceLoader.instance.LoadedTextures[8];

                //Inventory
                DraggedItemIndex = -1;
                DraggedItem = null;
                ModAPI.Log.Write("SETUP: Created Main Menu");
            }
            catch (Exception ex)
            {
                ModAPI.Log.Write("ERROR: Failure in start of Main Menu: " + ex.ToString());
            }
        }

        private void Update()
        {
            if (_openedMenu != OpenedMenuMode.Hud)
            {
                // LocalPlayer.FpCharacter.LockView(true);
            }
            Crouching = LocalPlayer.FpCharacter.crouching;
            if (ModAPI.Input.GetButtonDown("MenuToggle"))
            {
                MenuKeyPressAction();
            }

        }

        //Draws everything
        private void OnGUI()
        {
            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;



            mousepos = new Vector2(UnityEngine.Input.mousePosition.x, Screen.height - UnityEngine.Input.mousePosition.y);

            if (!ModSettings.DifficultyChoosen)
            {
                if (GameSetup.IsMpClient)
                {
                    DifficultySelectionClinet();
                }
                else
                {
                    DifficultySelectionHost();
                }
            }
            else
            {
                if (HUDStatStyle == null)
                {
                    HUDStatStyle = new GUIStyle(GUI.skin.label)
                    {
                        font = MainFont,
                        fontSize = Mathf.RoundToInt(33 * rr),
                        alignment = TextAnchor.LowerRight,
                        wordWrap = false,
                        clipping = TextClipping.Overflow,
                    };
                }

                if (MenuBtnStyle == null)
                {
                    MenuBtnStyle = new GUIStyle(GUI.skin.label)
                    {
                        font = MainFont,
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = Mathf.RoundToInt(37 * rr),
                        wordWrap = false,
                    };
                }

                try
                {


                    switch (_openedMenu)
                    {
                        case OpenedMenuMode.Main:
                            GUI.DrawTexture(wholeScreen, _black);

                            //GUI.Label(new Rect(wholeScreen.center, Vector2.one * 500), "Main");

                            DrawMain();
                            break;
                        case OpenedMenuMode.Inventory:
                            GUI.DrawTexture(wholeScreen, _black);
                            DrawInventory();

                            //GUI.Label(new Rect(wholeScreen.center, Vector2.one * 500), "Inventory");
                            break;
                        case OpenedMenuMode.Hud:
                            InventoryScrollAmount = 0;
                            DrawHUD();
                            break;
                        case OpenedMenuMode.Skills:
                            GUI.DrawTexture(wholeScreen, _black);
                            GUI.Label(new Rect(wholeScreen.center, Vector2.one * 500), "Skills");

                            break;
                        case OpenedMenuMode.Stats:
                            GUI.DrawTexture(wholeScreen, _black);
                            GUI.Label(new Rect(wholeScreen.center, Vector2.one * 500), "Stats");

                            break;
                        case OpenedMenuMode.Perks:
                            GUI.DrawTexture(wholeScreen, _black);
                            GUI.Label(new Rect(wholeScreen.center, Vector2.one * 500), "Perks");

                            break;
                    }
                    GUI.DrawTexture(wholeScreen, _blackTexture);
                }
                catch (Exception ex)
                {

                    ModAPI.Log.Write(ex.ToString());
                }
            }
        }

        #region MainMenuMethods
        //Draws main menu with buttons
        private void DrawMain()
        {
            try
            {
                if (MenuBtnStyle == null)
                {
                    MenuBtnStyle = new GUIStyle(GUI.skin.label)
                    {
                        fontSize = 30,
                        alignment = TextAnchor.MiddleCenter
                    };
                }
                if (isDragging)
                {
                    Inventory.Instance.DropItem(DraggedItemIndex);
                    DraggedItem = null;
                    DraggedItemIndex = -1;
                    isDragging = false;
                }

                Vector2 center = wholeScreen.center;
                Rect MiddleR = new Rect(Vector2.zero, new Vector2(300 * rr, 300 * rr))
                {
                    center = center
                };



                Rect r1 = new Rect(wholeScreen.center, new Vector2(Screen.height / 2, Screen.height / 2));
                Rect r2 = new Rect(r1);
                Rect r3 = new Rect(r1);
                Rect r4 = new Rect(r1);
                float Mindist = 375 / 1500;
                Mindist *= r1.width;
                MenuButton(Mindist, r1, OpenedMenuMode.Inventory, "Inventory", new Vector2(1, -1), ref Opacity1);
                r2.position = center - r1.size;
                MenuButton(Mindist, r2, OpenedMenuMode.Skills, "Skills", new Vector2(-1, 1), ref Opacity2);
                r3.position = center - new Vector2(0, r1.width);
                MenuButton(Mindist, r3, OpenedMenuMode.Stats, "Stats", Vector2.one, ref Opacity3);
                r4.position = center - new Vector2(r1.width, 0);
                MenuButton(Mindist, r4, OpenedMenuMode.Perks, "Perks", -Vector2.one, ref Opacity4);
                GUI.Label(MiddleR, "Level \n " + ModdedPlayer.instance.Level.ToString(), MenuBtnStyle);

            }
            catch (Exception ex)
            {

                ModAPI.Log.Write(ex.ToString());
            }
        }

        //Draws a single main menu button
        private void MenuButton(float mindist, Rect rect, OpenedMenuMode mode, string text, Vector2 Scale, ref float Opacity, float r = 1, float g = 1, float b = 1)
        {

            Matrix4x4 backupMatrix = GUI.matrix;
            GUIUtility.ScaleAroundPivot(Scale, rect.center);
            float dist = Vector2.Distance(mousepos, wholeScreen.center);
            GUI.DrawTexture(rect, Res.ResourceLoader.instance.LoadedTextures[1]);

            if (dist > mindist && dist < rect.height && rect.Contains(mousepos) && MenuInteractable)
            {
                Opacity = Mathf.Clamp01(Opacity + Time.deltaTime * OpacityGainSpeed);

                if (UnityEngine.Input.GetMouseButtonDown(0))
                {
                    StartCoroutine(FadeMenuSwitch(mode));
                }
            }
            else
            {
                Opacity = Mathf.Clamp01(Opacity - Time.deltaTime * OpacityGainSpeed);
            }
            if (Opacity > 0)
            {

                GUI.color = new Color(r, g, b, Opacity);
                GUI.DrawTexture(rect, Res.ResourceLoader.instance.LoadedTextures[2]);
                GUI.color = Color.white;

            }
            GUI.matrix = backupMatrix;
            GUI.Label(rect, text, MenuBtnStyle);

        }

        private void MenuKeyPressAction()
        {
            if (!MenuInteractable)
            {
                return;
            }

            if (_openedMenu == OpenedMenuMode.Main)
            {
                StartCoroutine(FadeMenuSwitch(OpenedMenuMode.Hud));
                LocalPlayer.FpCharacter.UnLockView();
                LocalPlayer.Inventory.EquipPreviousWeapon();


            }
            else if (_openedMenu == OpenedMenuMode.Hud)
            {
                StartCoroutine(FadeMenuSwitch(OpenedMenuMode.Main));
                LocalPlayer.FpCharacter.LockView(true);
                LocalPlayer.Inventory.StashEquipedWeapon(false);
                LocalPlayer.Inventory.StashLeftHand();
            }
            else
            {
                StartCoroutine(FadeMenuSwitch(OpenedMenuMode.Main));
                LocalPlayer.FpCharacter.LockView(true);
                LocalPlayer.Inventory.StashEquipedWeapon(false);
                LocalPlayer.Inventory.StashLeftHand();
            }
        }

        #endregion

        #region DifficultySelectionMethods
        private void DifficultySelectionHost()
        {
            LocalPlayer.FpCharacter.LockView(true);
            LocalPlayer.FpCharacter.MovementLocked = true;

            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _black);
            GUIStyle DifficultySelectionStyle = new GUIStyle(GUI.skin.button);
            GUIStyle DifSelNameStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.FloorToInt(30 * rr),
            };
            GUIStyle DifSelDescStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.FloorToInt(16 * rr),
            };
            for (int i = 0; i < 4; i++)
            {
                int ii = i + DiffSelPage * 4;
                if (DiffSel_Names.Length > ii)
                {


                    Rect r = new Rect(Screen.width / 4, Screen.height / 4, Screen.width / 8, Screen.height / 2);
                    r.x += i * r.width;
                    if (GUI.Button(r, "", DifficultySelectionStyle))
                    {
                        ModSettings.DifficultyChoosen = true;
                        Array values = Enum.GetValues(typeof(ModSettings.Difficulty));
                        ModSettings.difficulty = (ModSettings.Difficulty)values.GetValue(ii);
                        LocalPlayer.FpCharacter.UnLockView();
                        LocalPlayer.FpCharacter.MovementLocked = false;
                        return;
                    }
                    Rect name = new Rect(r);
                    name.height /= 5;
                    Rect desc = new Rect(r);
                    desc.height *= 0.8f;
                    desc.y += name.height;

                    GUI.Label(name, DiffSel_Names[ii], DifSelNameStyle);
                    GUI.Label(desc, DiffSel_Descriptions[ii], DifSelDescStyle);
                }
            }

            GUI.Label(new Rect(0, 0, Screen.width, Screen.height / 4), "Select difficulty", DifSelNameStyle);
            if (GUI.Button(new Rect(0, Screen.height / 2 - 100 * rr, 100 * rr, 200 * rr), "ArrL"))
            {
                DiffSelPage = Mathf.Clamp(DiffSelPage - 1, 0, 2);
            }
            if (GUI.Button(new Rect(Screen.width - 100 * rr, Screen.height / 2 - 100 * rr, 100 * rr, 200 * rr), "ArrR"))
            {
                DiffSelPage = Mathf.Clamp(DiffSelPage + 1, 0, 2);
            }

        }

        private void DifficultySelectionClinet()
        {
            LocalPlayer.FpCharacter.LockView(true);
            LocalPlayer.FpCharacter.MovementLocked = true;
            Rect r = new Rect(0, 0, Screen.width, Screen.height);
            GUI.DrawTexture(r, _black);
            GUI.Label(r, "Please wait for the host to choose a difficulty", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, font = MainFont, fontSize = Mathf.RoundToInt(50 * rr) });
            if (requestResendTime <= 0)
            {
                Network.NetworkManager.SendLine("AB", Network.NetworkManager.Target.OnlyServer);
                requestResendTime = 2;
            }
            else
            {
                requestResendTime -= Time.deltaTime;
            }
        }
        #endregion

        #region InventoryMethods
        private void DrawInventory()
        {
            Rect SlotsRect = new Rect(0, 0, Inventory.Width * slotDim.x, Screen.height);
            GUI.Box(SlotsRect, "Inventory", new GUIStyle(GUI.skin.box) { font = MainFont, fontSize = Mathf.RoundToInt(65 * rr) });


            for (int y = 0; y < Inventory.Height; y++)
            {
                for (int x = 0; x < Inventory.Width; x++)
                {
                    try
                    {
                        int index = y * Inventory.Width + x;

                        DrawInvSlot(new Rect(SlotsRect.x + slotDim.x * x, SlotsRect.y + slotDim.y * y + 80 * rr + InventoryScrollAmount, slotDim.x, slotDim.y), index);
                    }
                    catch (Exception ex)
                    {

                        ModAPI.Log.Write(ex.ToString());
                    }

                }
            }
            //PlayerSlots
            Rect eq = new Rect(SlotsRect.xMax, 0, 400 * rr, 700 * rr);
            GUI.Box(eq, "Equipment", new GUIStyle(GUI.skin.box) { font = MainFont, fontSize = Mathf.RoundToInt(65 * rr) });
            Rect head = new Rect(Vector2.zero, slotDim)
            {
                center = eq.center
            };
            head.y -= 2 * head.height;

            Rect chest = new Rect(head);
            chest.y += chest.height + 35 * rr;

            Rect pants = new Rect(chest);
            pants.y += pants.height + 35 * rr;

            Rect boots = new Rect(pants);
            boots.y += boots.height + 35 * rr;

            Rect shoulders = new Rect(chest);
            shoulders.position += new Vector2(-chest.width, -chest.height / 2);

            Rect tallisman = new Rect(chest);
            tallisman.position += new Vector2(chest.width, -chest.height / 2);

            Rect gloves = new Rect(shoulders);
            gloves.y += gloves.height + 35 * rr;

            Rect bracer = new Rect(tallisman);
            bracer.y += bracer.height + 35 * rr;

            Rect ringR = new Rect(bracer);
            ringR.position += new Vector2(chest.width / 2, chest.height);

            Rect ringL = new Rect(gloves);
            ringL.position += new Vector2(-chest.width / 2, chest.height);

            Rect weapon = new Rect(ringL);
            weapon.y += weapon.height * 1.5f + 35 * rr;

            Rect offhand = new Rect(ringR);
            offhand.y += offhand.height * 1.5f + 35 * rr;

            DrawInvSlot(head, -2, "Head");
            DrawInvSlot(chest, -3, "Chest");
            DrawInvSlot(pants, -4, "Pants");
            DrawInvSlot(boots, -5, "Boots");
            DrawInvSlot(shoulders, -6, "Shoulders");
            DrawInvSlot(gloves, -7, "Gloves");
            DrawInvSlot(tallisman, -8, "Tallisman");
            DrawInvSlot(bracer, -9, "Bracer");
            DrawInvSlot(ringR, -10, "Ring");
            DrawInvSlot(ringL, -11, "Ring");
            DrawInvSlot(weapon, -12, "Main hand");
            DrawInvSlot(offhand, -13, "Offhand");




            if (SelectedItem != -1)
            {

                DrawItemInfo(itemPos, Inventory.Instance.ItemList[SelectedItem]);
            }
            //ModAPI.Console.Write(DraggedItemIndex.ToString());

            if (isDragging)
            {
                Rect r = new Rect(mousepos, slotDim);
                GUI.color = new Color(1, 1, 1, 0.5f);
                GUI.DrawTexture(r, DraggedItem.icon);
                GUI.color = new Color(1, 1, 1, 1);

                if (UnityEngine.Input.GetMouseButtonUp(0))
                {
                    Inventory.Instance.DropItem(DraggedItemIndex);
                    DraggedItem = null;
                    DraggedItemIndex = -1;
                    isDragging = false;
                }
            }
        }

        private void DrawItemInfo(Vector2 pos, Item item)
        {
            float width = 390 * rr;
            Vector2 originalPos = pos;
            pos.x += 5 * rr;
            if (pos.x + width > Screen.width)
            {
                pos.x -= width + slotDim.x;
                pos.x -= 5 * rr;
            }


            Rect descriptionBox = new Rect(originalPos, new Vector2(width + 10 * rr, 500 * rr));
            Rect ItemNameRect = new Rect(pos.x, pos.y, width, 50 * rr);
            GUIStyle ItemNameStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = Mathf.RoundToInt(45 * rr), font = MainFont };
            float y = 70 + pos.y;
            Rect[] StatRects = new Rect[item.Stats.Count];
            GUIStyle StatNameStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontSize = Mathf.RoundToInt(40 * rr), font = MainFont };
            GUIStyle StatValueStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleRight, fontSize = Mathf.RoundToInt(40 * rr), font = MainFont };

            for (int i = 0; i < StatRects.Length; i++)
            {
                StatRects[i] = new Rect(pos.x, y, width, 40 * rr);
                y += 42 * rr;
            }
            y += 35 * rr;

            Rect LevelAndTypeRect = new Rect(pos.x, y, width, 35 * rr);
            GUIStyle TypeStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperLeft, fontSize = Mathf.RoundToInt(25 * rr), font = MainFont };
            GUIStyle LevelStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.LowerRight, fontSize = Mathf.RoundToInt(28 * rr), font = MainFont, fontStyle = FontStyle.Italic };
            y += 35 * rr;

            GUIStyle DescriptionStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperLeft, fontSize = Mathf.RoundToInt(30 * rr), font = MainFont };
            GUIStyle LoreStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperLeft, fontSize = Mathf.RoundToInt(27 * rr), font = MainFont, fontStyle = FontStyle.Italic };
            GUIStyle TooltipStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperLeft, fontSize = Mathf.RoundToInt(30 * rr), font = MainFont, fontStyle = FontStyle.Bold };

            Rect DescrRect = new Rect(pos.x, y, width, DescriptionStyle.CalcHeight(new GUIContent(item.description), width)); y += DescrRect.height;
            y += 35 * rr;

            Rect LoreRect = new Rect(pos.x, y, width, LoreStyle.CalcHeight(new GUIContent(item.lore), width)); y += LoreRect.height;
            y += 35 * rr;

            Rect toolTipTitleRect = new Rect(pos.x, y, width, TooltipStyle.fontSize + 2); y += toolTipTitleRect.height;
            Rect toolTipRect = new Rect(pos.x, y, width, TooltipStyle.CalcHeight(new GUIContent(item.tooltip), width)); y += toolTipRect.height;
            descriptionBox.height = toolTipRect.yMax - descriptionBox.y + 5;
            if (descriptionBox.yMax > Screen.height)
            {

            }
            GUI.DrawTexture(descriptionBox, _black);
            GUI.color = RarityColors[item.Rarity];
            GUI.Label(ItemNameRect, item.name, ItemNameStyle);
            for (int i = 0; i < StatRects.Length; i++)
            {
                GUI.color = RarityColors[item.Stats[i].Rarity];
                GUI.Label(StatRects[i], item.Stats[i].Name, StatNameStyle);
                GUI.Label(StatRects[i], Mathf.Round(item.Stats[i].Amount).ToString(), StatValueStyle);

            }
            GUI.color = Color.white;
            switch (item._itemType)
            {
                case BaseItem.ItemType.Shield:
                    GUI.Label(LevelAndTypeRect, "Shield", TypeStyle);
                    break;
                case BaseItem.ItemType.Offhand:
                    GUI.Label(LevelAndTypeRect, "Offhand", TypeStyle);
                    break;
                case BaseItem.ItemType.Weapon:
                    GUI.Label(LevelAndTypeRect, "Weapon", TypeStyle);
                    break;
                case BaseItem.ItemType.Other:
                    GUI.Label(LevelAndTypeRect, "Other", TypeStyle);
                    break;
                case BaseItem.ItemType.Material:
                    GUI.Label(LevelAndTypeRect, "Material", TypeStyle);
                    break;
                case BaseItem.ItemType.Helmet:
                    GUI.Label(LevelAndTypeRect, "Helmet", TypeStyle);
                    break;
                case BaseItem.ItemType.Boot:
                    GUI.Label(LevelAndTypeRect, "Boots", TypeStyle);
                    break;
                case BaseItem.ItemType.Pants:
                    GUI.Label(LevelAndTypeRect, "Pants", TypeStyle);
                    break;
                case BaseItem.ItemType.ChestArmor:
                    GUI.Label(LevelAndTypeRect, "Chest armor", TypeStyle);
                    break;
                case BaseItem.ItemType.ShoulderArmor:
                    GUI.Label(LevelAndTypeRect, "Shoulder armor", TypeStyle);
                    break;
                case BaseItem.ItemType.Glove:
                    GUI.Label(LevelAndTypeRect, "Gloves", TypeStyle);
                    break;
                case BaseItem.ItemType.Bracer:
                    GUI.Label(LevelAndTypeRect, "Bracers", TypeStyle);
                    break;
                case BaseItem.ItemType.Amulet:
                    GUI.Label(LevelAndTypeRect, "Amulet", TypeStyle);
                    break;
                case BaseItem.ItemType.Ring:
                    GUI.Label(LevelAndTypeRect, "Ring", TypeStyle);
                    break;
            }
            if (item.level <= ModdedPlayer.instance.Level)
            {
                //GUI.color = Color.white;
                GUI.Label(LevelAndTypeRect, "Level " + item.level, LevelStyle);
            }
            else
            {
                GUI.color = Color.red;
                GUI.Label(LevelAndTypeRect, "Level " + item.level, LevelStyle);
            }
            GUI.color = Color.white;
            GUI.Label(DescrRect, item.description, DescriptionStyle);
            GUI.Label(LoreRect, item.lore, LoreStyle);
            GUI.Label(toolTipTitleRect, "Tooltip:", TooltipStyle);
            GUI.Label(toolTipRect, item.tooltip, TooltipStyle);

        }

        private void DrawInvSlot(Rect r, int index)
        {
            Color frameColor = Color.black;
            GUI.DrawTexture(r, Res.ResourceLoader.instance.LoadedTextures[12]);




            if (Inventory.Instance.ItemList[index] != null)
            {
                frameColor = RarityColors[Inventory.Instance.ItemList[index].Rarity];
                if (Inventory.Instance.ItemList[index].icon != null)
                {

                    Rect itemRect = new Rect(r);
                    float f = r.width * 0.15f;
                    itemRect.width -= f;
                    itemRect.height -= f;
                    itemRect.x += f / 2;
                    itemRect.y += f / 2;

                    if (DraggedItemIndex != -1)
                    {
                        if (DraggedItemIndex == index)
                        {
                            GUI.color = new Color(1, 1, 1, 0.5f);

                        }
                    }else if(ModdedPlayer.instance.Level > Inventory.Instance.ItemList[index].level)
                    {
                        GUI.color = new Color(1, 0, 0, 0.5f);
                    }

                    GUI.DrawTexture(itemRect, Inventory.Instance.ItemList[index].icon);
                    GUI.color = new Color(1, 1, 1, 1);
                    if (isDragging)
                    {
                        if (r.Contains(mousepos) && UnityEngine.Input.GetMouseButtonUp(0))
                        {
                            SelectedItem = index;
                            if (index < -1)
                            {
                                bool canPlace = false;
                                switch (index)
                                {
                                    case -2:
                                        if (DraggedItem._itemType == BaseItem.ItemType.Helmet)
                                        {
                                            canPlace = true;
                                        }

                                        break;
                                    case -3:
                                        if (DraggedItem._itemType == BaseItem.ItemType.ChestArmor)
                                        {
                                            canPlace = true;
                                        }

                                        break;
                                    case -4:
                                        if (DraggedItem._itemType == BaseItem.ItemType.Pants)
                                        {
                                            canPlace = true;
                                        }

                                        break;
                                    case -5:
                                        if (DraggedItem._itemType == BaseItem.ItemType.Boot)
                                        {
                                            canPlace = true;
                                        }

                                        break;
                                    case -6:
                                        if (DraggedItem._itemType == BaseItem.ItemType.ShoulderArmor)
                                        {
                                            canPlace = true;
                                        }

                                        break;
                                    case -7:
                                        if (DraggedItem._itemType == BaseItem.ItemType.Glove)
                                        {
                                            canPlace = true;
                                        }

                                        break;
                                    case -8:
                                        if (DraggedItem._itemType == BaseItem.ItemType.Amulet)
                                        {
                                            canPlace = true;
                                        }

                                        break;
                                    case -9:
                                        if (DraggedItem._itemType == BaseItem.ItemType.Bracer)
                                        {
                                            canPlace = true;
                                        }

                                        break;
                                    case -10:
                                        if (DraggedItem._itemType == BaseItem.ItemType.Ring)
                                        {
                                            canPlace = true;
                                        }

                                        break;
                                    case -11:
                                        if (DraggedItem._itemType == BaseItem.ItemType.Ring)
                                        {
                                            canPlace = true;
                                        }

                                        break;
                                    case -12:
                                        if (DraggedItem._itemType == BaseItem.ItemType.Weapon)
                                        {
                                            canPlace = true;
                                        }

                                        break;
                                    case -13:
                                        if (DraggedItem._itemType == BaseItem.ItemType.Weapon || DraggedItem._itemType == BaseItem.ItemType.Offhand || DraggedItem._itemType == BaseItem.ItemType.Shield)
                                        {
                                            canPlace = true;
                                        }

                                        break;
                                }
                                if (canPlace)
                                {
                                    Item backup = Inventory.Instance.ItemList[index];
                                    Inventory.Instance.ItemList[index] = DraggedItem;
                                    Inventory.Instance.ItemList[DraggedItemIndex] = backup;
                                    DraggedItem = null;
                                    DraggedItemIndex = -1;
                                    isDragging = false;
                                }
                                else
                                {
                                    Inventory.Instance.ItemList[DraggedItemIndex] = DraggedItem;
                                    DraggedItem = null;
                                    DraggedItemIndex = -1;
                                }
                            }
                            else
                            {
                                if (DraggedItem.ID != Inventory.Instance.ItemList[index].ID
                                 || DraggedItem.Amount == DraggedItem.StackSize
                                 || Inventory.Instance.ItemList[index].Amount == Inventory.Instance.ItemList[index].StackSize
                                 || (Inventory.Instance.ItemList[index].StackSize <= 1 && DraggedItem.StackSize <= 1))
                                {
                                    //replace items
                                    Item backup = Inventory.Instance.ItemList[index];
                                    Inventory.Instance.ItemList[index] = DraggedItem;
                                    Inventory.Instance.ItemList[DraggedItemIndex] = backup;
                                    DraggedItem = null;
                                    DraggedItemIndex = -1;
                                    isDragging = false;
                                }
                                else
                                {
                                    //stack items
                                    int i = DraggedItem.Amount + Inventory.Instance.ItemList[index].Amount - DraggedItem.StackSize;
                                    if (i > 0)  //too much to stack completely
                                    {
                                        Inventory.Instance.ItemList[index].Amount = Inventory.Instance.ItemList[index].StackSize;
                                        Inventory.Instance.ItemList[DraggedItemIndex].Amount = i;
                                        DraggedItem = null;
                                        DraggedItemIndex = -1;
                                        isDragging = false;
                                    }
                                    else //enough to stack completely
                                    {
                                        Inventory.Instance.ItemList[index].Amount += DraggedItem.Amount;
                                        Inventory.Instance.ItemList[DraggedItemIndex] = null;
                                        DraggedItem = null;
                                        DraggedItemIndex = -1;
                                        isDragging = false;
                                    }


                                }
                            }
                        }
                    }
                    else
                    {
                        if (r.Contains(mousepos) && UnityEngine.Input.GetMouseButtonDown(0))
                        {

                            isDragging = true;
                            DraggedItem = Inventory.Instance.ItemList[index];
                            DraggedItemIndex = index;

                        }
                    }
                }
            }
            else
            {
                if (isDragging)
                {
                    if (r.Contains(mousepos) && UnityEngine.Input.GetMouseButtonUp(0))
                    {
                        isDragging = false;
                        if (index < -1)
                        {
                            bool canPlace = false;
                            switch (index)
                            {
                                case -2:
                                    if (DraggedItem._itemType == BaseItem.ItemType.Helmet)
                                    {
                                        canPlace = true;
                                    }

                                    break;
                                case -3:
                                    if (DraggedItem._itemType == BaseItem.ItemType.ChestArmor)
                                    {
                                        canPlace = true;
                                    }

                                    break;
                                case -4:
                                    if (DraggedItem._itemType == BaseItem.ItemType.Pants)
                                    {
                                        canPlace = true;
                                    }

                                    break;
                                case -5:
                                    if (DraggedItem._itemType == BaseItem.ItemType.Boot)
                                    {
                                        canPlace = true;
                                    }

                                    break;
                                case -6:
                                    if (DraggedItem._itemType == BaseItem.ItemType.ShoulderArmor)
                                    {
                                        canPlace = true;
                                    }

                                    break;
                                case -7:
                                    if (DraggedItem._itemType == BaseItem.ItemType.Glove)
                                    {
                                        canPlace = true;
                                    }

                                    break;
                                case -8:
                                    if (DraggedItem._itemType == BaseItem.ItemType.Amulet)
                                    {
                                        canPlace = true;
                                    }

                                    break;
                                case -9:
                                    if (DraggedItem._itemType == BaseItem.ItemType.Bracer)
                                    {
                                        canPlace = true;
                                    }

                                    break;
                                case -10:
                                    if (DraggedItem._itemType == BaseItem.ItemType.Ring)
                                    {
                                        canPlace = true;
                                    }

                                    break;
                                case -11:
                                    if (DraggedItem._itemType == BaseItem.ItemType.Ring)
                                    {
                                        canPlace = true;
                                    }

                                    break;
                                case -12:
                                    if (DraggedItem._itemType == BaseItem.ItemType.Weapon)
                                    {
                                        canPlace = true;
                                    }

                                    break;
                                case -13:
                                    if (DraggedItem._itemType == BaseItem.ItemType.Weapon || DraggedItem._itemType == BaseItem.ItemType.Offhand || DraggedItem._itemType == BaseItem.ItemType.Shield)
                                    {
                                        canPlace = true;
                                    }

                                    break;
                            }
                            if (canPlace)
                            {
                                Inventory.Instance.ItemList[index] = DraggedItem;
                                Inventory.Instance.ItemList[DraggedItemIndex] = null;
                                DraggedItem = null;
                                DraggedItemIndex = -1;
                            }
                            else
                            {
                                Inventory.Instance.ItemList[index] = null;
                                Inventory.Instance.ItemList[DraggedItemIndex] = DraggedItem;
                                DraggedItem = null;
                                DraggedItemIndex = -1;
                            }
                        }

                        else if (DraggedItemIndex != index)
                        {
                            Inventory.Instance.ItemList[index] = DraggedItem;
                            Inventory.Instance.ItemList[DraggedItemIndex] = null;
                            DraggedItem = null;
                            DraggedItemIndex = -1;

                        }
                    }
                }
            }
            GUI.color = frameColor;
            GUI.DrawTexture(r, Res.ResourceLoader.instance.LoadedTextures[13]);
            GUI.color = Color.white;
            if (r.Contains(mousepos))
            {
                if (Inventory.Instance.ItemList[index] != null)
                {
                    itemPos = r.position + r.width * Vector2.right;
                    SelectedItem = index;
                }
                else
                {
                    SelectedItem = -1;
                }
            }
        }

        private void DrawInvSlot(Rect r, int index, string title)
        {


            Rect TitleR = new Rect(r.x, r.y - 35 * rr, r.width, 35 * rr);
            GUI.Label(TitleR, title, new GUIStyle(GUI.skin.box) { font = MainFont, fontSize = Mathf.RoundToInt(29 * rr),wordWrap = false,alignment = TextAnchor.MiddleCenter});
            DrawInvSlot(r, index);

        }
        #endregion

        #region HUDMethods
        private void DrawHUD()
        {
            try
            {


                if (HideHud)
                {
                    return;
                }

                GUI.color = Color.blue;
                GUI.Label(HUDenergyLabelRect, Mathf.Floor(LocalPlayer.Stats.Stamina) + "/" + ModdedPlayer.instance.MaxEnergy, HUDStatStyle);
                GUI.color = new Color(0, 0.8f, 0.1f);

                GUI.Label(HUDHealthLabelRect, Mathf.Floor(LocalPlayer.Stats.Health) + "/" + ModdedPlayer.instance.MaxHealth, HUDStatStyle);
                GUI.color = Color.white;

                float SquareSize = 100 * rr;
                for (int i = 0; i < SpellCaster.SpellCount; i++)
                {
                    Rect r = new Rect(
                        Screen.width / 2f - (SquareSize * SpellCaster.SpellCount / 2f) + i * SquareSize,
                        Screen.height - SquareSize,
                        SquareSize,
                        SquareSize
                        );


                    GUI.DrawTexture(r, _SpellBG);

                    if (SpellCaster.instance.infos[i].spell == null || SpellCaster.instance.infos[i].spell.icon == null)
                    {


                    }
                    else
                    {
                        GUI.DrawTexture(r, SpellCaster.instance.infos[i].spell.icon);
                        if (!SpellCaster.instance.infos[i].spell.Bought)
                        {

                        }
                        else
                        {
                            Rect fillr = new Rect(r);
                            float f = SpellCaster.instance.infos[i].Cooldown / SpellCaster.instance.infos[i].spell.BaseCooldown;
                            fillr.height *= f;
                            fillr.y += SquareSize * (1 - f);
                            GUI.DrawTexture(fillr, _SpellCoolDownFill, ScaleMode.ScaleAndCrop);

                        }
                    }
                    GUI.DrawTexture(r, _SpellFrame);

                }
                float width = SpellCaster.SpellCount * SquareSize;
                float height = width * 0.1f;
                float combatHeight = width * 63 / 1500;
                Rect XPbar = new Rect(Screen.width / 2f - (SquareSize * SpellCaster.SpellCount / 2f), Screen.height - SquareSize - height, width, height);
                Rect XPbarFill = new Rect(XPbar);
                XPbarFill.width *= (float)ModdedPlayer.instance.ExpCurrent / ModdedPlayer.instance.ExpGoal;
                Rect CombatBar = new Rect(XPbar.x, XPbar.y - combatHeight, SpellCaster.SpellCount * SquareSize * (ModdedPlayer.instance.TimeUntillMassacreReset / ModdedPlayer.instance.MaxMassacreTime), combatHeight);
                GUI.DrawTexture(XPbar, _expBarBackgroundTex, ScaleMode.ScaleAndCrop, true, 1500 / 150);
                GUI.DrawTexture(XPbarFill, _expBarFillTex, ScaleMode.ScaleAndCrop, true, 1500 / 150);
                GUI.DrawTexture(XPbar, _expBarFrameTex, ScaleMode.ScaleAndCrop, true, 1500 / 150);
                GUIStyle expInfoStyle = new GUIStyle(GUI.skin.label)
                {
                    font = MainFont,
                    fontSize = Mathf.RoundToInt(22 * rr),
                    alignment = TextAnchor.LowerCenter,
                    wordWrap = false,
                    clipping = TextClipping.Overflow,
                };
                GUI.Label(XPbar, ModdedPlayer.instance.ExpCurrent + "/" + ModdedPlayer.instance.ExpGoal + "       " + Mathf.Floor(ModdedPlayer.instance.ExpCurrent * 100 / ModdedPlayer.instance.ExpGoal) + "%", expInfoStyle);
                GUI.DrawTexture(CombatBar, _combatDurationTex, ScaleMode.ScaleAndCrop, true, 1500 / 63);



                if (LocalPlayer.FpCharacter.crouching)
                {
                    RaycastHit[] hits = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward, 1000);
                    int enemyHit = -1;
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i].transform.CompareTag("enemyCollide"))
                        {
                            enemyHit = i;
                            break;
                        }
                    }
                    if (enemyHit != -1)
                    {
                        ScanTime += Time.deltaTime;
                        if (hits[enemyHit].transform.root == scannedTransform)
                        {
                            ClinetEnemyProgression cp = null;
                            if (BoltNetwork.isRunning && scannedEntity != null)
                            {
                                cp = EnemyManager.GetCP(scannedEntity);
                            }
                            else
                            {
                                cp = EnemyManager.GetCP(hits[enemyHit].transform.root);
                            }
                            if (cp != null)
                            {

                                Rect scanRect = new Rect(0, 0, 60 * rr, 60 * rr)
                                {
                                    center = wholeScreen.center
                                };
                                ScanRotation += Time.deltaTime * 45;
                                Matrix4x4 matrix4X4 = GUI.matrix;
                                GUIUtility.RotateAroundPivot(ScanRotation, scanRect.center);
                                GUI.DrawTexture(scanRect, ResourceLoader.instance.LoadedTextures[24]);

                                GUI.matrix = matrix4X4;

                                GUIStyle infoStyle = new GUIStyle(GUI.skin.label)
                                {
                                    font = MainFont,
                                    fontSize = Mathf.RoundToInt(35 * rr),
                                    alignment = TextAnchor.MiddleRight,
                                    wordWrap = false,
                                    clipping = TextClipping.Overflow,
                                };

                                Vector2 origin = wholeScreen.center;
                                origin.y -= 350 * rr;
                                origin.x += 100 * rr;
                                float y = 0;
                                DrawScannedEnemyLabel(cp.EnemyName, new Rect(origin.x, origin.y + y, 250 * rr, 66 * rr), infoStyle);
                                y += rr * 65;
                                DrawScannedEnemyLabel(cp.Level + " :LEVEL", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                y += rr * 65;
                                if (ScanTime > 1.5f)
                                {
                                    DrawScannedEnemyLabel(cp.Health + "/" + cp.MaxHealth + " :HEALTH", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                    y += rr * 65;
                                }
                                if (ScanTime > 3f)
                                {
                                    DrawScannedEnemyLabel(cp.Armor + " :ARMOR", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                    y += rr * 65;
                                    if (cp.ArmorReduction != 0)
                                    {
                                        GUI.color = Color.red;
                                        DrawScannedEnemyLabel("➖ " + cp.ArmorReduction + " :ARMOR DEBUFF", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                        y += rr * 65;
                                        GUI.color = Color.white;
                                    }
                                }
                                if (ScanTime > 4.5f)
                                {
                                    DrawScannedEnemyLabel(cp.ExpBounty + " :BOUNTY", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                    y += rr * 85;
                                }
                                if (ScanTime > 6f)
                                {
                                    if (cp.Affixes.Length > 0)
                                    {

                                        DrawScannedEnemyLabel("☠️     ", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(75 * rr), alignment = TextAnchor.MiddleRight });
                                        y += rr * 65;

                                        Array arr = Enum.GetValues(typeof(EnemyProgression.Abilities));
                                        foreach (int i in cp.Affixes)
                                        {
                                            EnemyProgression.Abilities ability = (EnemyProgression.Abilities)arr.GetValue(i);
                                            switch (ability)
                                            {
                                                case EnemyProgression.Abilities.SteadFest:
                                                    DrawScannedEnemyLabel("Stead fest", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                                    y += rr * 65;
                                                    break;
                                                case EnemyProgression.Abilities.BossSteadFest:
                                                    DrawScannedEnemyLabel("Boss ability: Stead fest", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                                    y += rr * 65;
                                                    break;
                                                case EnemyProgression.Abilities.EliteSteadFest:
                                                    DrawScannedEnemyLabel("Elite ability: Stead fest", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                                    y += rr * 65;
                                                    break;
                                                case EnemyProgression.Abilities.Molten:
                                                    DrawScannedEnemyLabel("Molten", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                                    y += rr * 65;
                                                    break;
                                                case EnemyProgression.Abilities.FreezingAura:
                                                    DrawScannedEnemyLabel("Absolute zero", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                                    y += rr * 65;
                                                    break;
                                                case EnemyProgression.Abilities.FireAura:
                                                    DrawScannedEnemyLabel("Radiance", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                                    y += rr * 65;
                                                    break;
                                                case EnemyProgression.Abilities.Rooting:
                                                    DrawScannedEnemyLabel("Nature's wrath", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                                    y += rr * 65;
                                                    break;
                                                case EnemyProgression.Abilities.BlackHole:
                                                    DrawScannedEnemyLabel("Gravity manipulation", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                                    y += rr * 65;
                                                    break;
                                                case EnemyProgression.Abilities.Mines:
                                                    DrawScannedEnemyLabel("Trapper", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                                    y += rr * 65;
                                                    break;
                                                case EnemyProgression.Abilities.Juggernaut:
                                                    DrawScannedEnemyLabel("Unstoppable force", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                                    y += rr * 65;
                                                    break;
                                                case EnemyProgression.Abilities.Huge:
                                                    DrawScannedEnemyLabel("Gargantuan", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                                    y += rr * 65;
                                                    break;
                                                case EnemyProgression.Abilities.Tiny:
                                                    DrawScannedEnemyLabel("Tiny", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                                    y += rr * 65;
                                                    break;
                                                case EnemyProgression.Abilities.ExtraDamage:
                                                    DrawScannedEnemyLabel("Extra deadly", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                                    y += rr * 65;
                                                    break;
                                                case EnemyProgression.Abilities.ExtraHealth:
                                                    DrawScannedEnemyLabel("Extra tough", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                                    y += rr * 65;
                                                    break;
                                                case EnemyProgression.Abilities.Illusionist:
                                                    DrawScannedEnemyLabel("Illusionist", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                                    y += rr * 65;
                                                    GUILayout.Label("Illusionist", infoStyle);
                                                    break;
                                                case EnemyProgression.Abilities.Blink:
                                                    DrawScannedEnemyLabel("Warping", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                                    y += rr * 65;
                                                    break;
                                                case EnemyProgression.Abilities.Thunder:
                                                    DrawScannedEnemyLabel("Thunder", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                                    y += rr * 65;
                                                    break;
                                                case EnemyProgression.Abilities.RainEmpowerement:
                                                    DrawScannedEnemyLabel("Rain empowerment", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                                    y += rr * 65;
                                                    break;
                                                case EnemyProgression.Abilities.Shielding:
                                                    DrawScannedEnemyLabel("Refraction", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                                    y += rr * 65;
                                                    break;
                                                case EnemyProgression.Abilities.Meteor:
                                                    DrawScannedEnemyLabel("Chaos meteor", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                                    y += rr * 65;
                                                    break;
                                                case EnemyProgression.Abilities.RockTosser:
                                                    DrawScannedEnemyLabel("Boulder tosser", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                                    y += rr * 65;
                                                    break;
                                                case EnemyProgression.Abilities.DoubleLife:
                                                    DrawScannedEnemyLabel("Undead", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                                    y += rr * 65;
                                                    break;
                                                case EnemyProgression.Abilities.Laser:
                                                    DrawScannedEnemyLabel("Plasma cannon", new Rect(origin.x, origin.y + y, 250 * rr, 65 * rr), infoStyle);
                                                    y += rr * 65;
                                                    break;
                                            }

                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            scannedTransform = hits[enemyHit].transform.root;
                            scannedEntity = scannedTransform.GetComponentInChildren<BoltEntity>();
                            if (scannedEntity == null)
                            {
                                scannedEntity = scannedTransform.GetComponent<BoltEntity>();
                            }
                            ScanTime = 0;

                        }
                    }
                }
                else
                {
                    ScanTime = 0;
                }


            }
            catch (Exception ex)
            {

                ModAPI.Log.Write("ERROR: FAILURE IN DRAW HUD METHOD \n " + ex.ToString());
            }
        }

        private void DrawScannedEnemyLabel(string content, Rect r, GUIStyle style)
        {
            GUI.DrawTexture(r, Res.ResourceLoader.instance.LoadedTextures[25]);
            Rect rOffset = new Rect(r);
            rOffset.x -= 30;
            GUI.Label(rOffset, content, style);

        }
        #endregion

        //Does fade out fade in transision between menus
        private IEnumerator FadeMenuSwitch(OpenedMenuMode mode)
        {
            MenuInteractable = false;
            float alpha = 0;
            while (alpha < 1)
            {
                alpha = Mathf.Clamp(alpha + Time.deltaTime * DarkeningSpeed, 0, 1);
                _blackTexture.SetPixel(0, 0, new Color(0, 0, 0, alpha));
                _blackTexture.Apply();
                yield return null;
            }

            _openedMenu = mode;
            yield return null;
            while (alpha > 0)
            {
                alpha = Mathf.Clamp(alpha - Time.deltaTime * DarkeningSpeed, 0, 1);
                _blackTexture.SetPixel(0, 0, new Color(0, 0, 0, alpha));
                _blackTexture.Apply();
                yield return null;
            }
            MenuInteractable = true;
        }

    }
}