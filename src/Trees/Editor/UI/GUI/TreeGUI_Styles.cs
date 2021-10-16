using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.GUI
{
    [InitializeOnLoad]
    public static partial class TreeGUI
    {
        public static class Styles
        {
            private static GUIStyle blackLabel;

            private static GUIStyle boldLabel;
            private static GUIStyle boldLabelCentered;
            private static GUIStyle boldTitle;
            private static GUIStyle boldTitleCentered;
            private static GUIStyle boldTitleRight;
            private static GUIStyle bottomBoxPadding;
            private static GUIStyle boxContainer;
            private static GUIStyle boxHeaderStyle;
            private static GUIStyle button;
            private static GUIStyle buttonLoud;
            private static GUIStyle buttonFlexH;
            private static GUIStyle buttonFlexV;
            private static GUIStyle buttonLeft;
            private static GUIStyle buttonLeftSelected;
            private static GUIStyle buttonMid;
            private static GUIStyle buttonMidSelected;
            private static GUIStyle buttonRight;
            private static GUIStyle buttonRightSelected;
            private static GUIStyle buttonSelected;
            private static GUIStyle cardStyle;
            private static GUIStyle centeredBlackMiniLabel;
            private static GUIStyle centeredGreyMiniLabel;
            private static GUIStyle centeredTextField;
            private static GUIStyle centeredWhiteMiniLabel;
            private static GUIStyle colorFieldBackground;
            private static GUIStyle containerOuterShadow;
            private static GUIStyle containerOuterShadowGlow;
            private static GUIStyle contentPadding;
            private static GUIStyle detailedMessageBox;
            private static GUIStyle dropdownField;
            private static GUIStyle dropdownPopup;
            private static GUIStyle editBox;
            private static GUIStyle foldout;
            private static GUIStyle iconButton;
            private static GUIStyle label;
            private static GUIStyle labelCentered;
            private static GUIStyle leftAlignedCenteredLabel;
            private static GUIStyle leftAlignedGreyMiniLabel;
            private static GUIStyle leftAlignedWhiteMiniLabel;
            private static GUIStyle listItem;
            private static GUIStyle menuButtonBackground;
            private static GUIStyle messageBox;
            private static GUIStyle miniButton;
            private static GUIStyle miniButtonLeft;
            private static GUIStyle miniButtonLeftSelected;
            private static GUIStyle miniButtonMid;
            private static GUIStyle miniButtonMidSelected;
            private static GUIStyle miniButtonRight;
            private static GUIStyle miniButtonRightSelected;
            private static GUIStyle miniButtonSelected;
            private static GUIStyle moduleHeader;
            private static GUIStyle multiLineCenteredLabel;
            private static GUIStyle multiLineLabel;
            private static GUIStyle[] nodeBoxes;
            private static GUIStyle nodeLabelTop;
            private static GUIStyle none;
            private static GUIStyle odinEditorWrapper;
            private static GUIStyle paddingLessBox;
            private static GUIStyle paneOptions;
            private static GUIStyle pinLabel;
            private static GUIStyle popup;
            private static GUIStyle propertyMargin;
            private static GUIStyle propertyPadding;
            private static GUIStyle richTextLabel;
            private static GUIStyle rightAlignedGreyMiniLabel;
            private static GUIStyle rightAlignedWhiteMiniLabel;
            private static GUIStyle sectionHeader;
            private static GUIStyle sectionHeaderCentered;
            private static GUIStyle smallTitle;
            private static GUIStyle smallTitleCentered;
            private static GUIStyle smallTitleRight;
            private static GUIStyle subtitle;
            private static GUIStyle subtitleCentered;
            private static GUIStyle subtitleRight;
            private static GUIStyle tagButton;
            private static GUIStyle title;
            private static GUIStyle titleCentered;
            private static GUIStyle titleRight;
            private static GUIStyle toggleGroupBackground;
            private static GUIStyle toggleGroupCheckbox;
            private static GUIStyle toggleGroupPadding;
            private static GUIStyle toggleGroupTitleBg;
            private static GUIStyle toolbarBackground;
            private static GUIStyle toolbarButton;
            private static GUIStyle toolbarButtonSelected;
            private static GUIStyle toolbarSeachCancelButton;
            private static GUIStyle toolbarSeachTextField;
            private static GUIStyle toolbarTab;
            private static GUIStyle treeLabel;
            private static GUIStyle variantToobar;
            private static GUIStyle whiteLabel;

            public static GUIStyle DropdownField
            {
                get
                {
                    if (dropdownField == null)
                    {
                        dropdownField = new GUIStyle(EditorStyles.numberField)
                        {
                            fixedHeight = 24

                            //padding = new RectOffset(0, 0, 0, 0)
                        };
                    }

                    return dropdownField;
                }
            }

            public static GUIStyle DropdownPopup
            {
                get
                {
                    if (dropdownPopup == null)
                    {
                        dropdownPopup = new GUIStyle(EditorStyles.popup)
                        {
                            fontSize = 12

                            //padding = new RectOffset(0, 0, 0, 0)
                        };
                    }

                    return dropdownPopup;
                }
            }

            public static GUIStyle BoldTitle
            {
                get
                {
                    if (boldTitle == null)
                    {
                        boldTitle = new GUIStyle(Title)
                        {
                            fontStyle = FontStyle.Bold, padding = new RectOffset(0, 0, 0, 0)
                        };
                    }

                    return boldTitle;
                }
            }

            public static GUIStyle BoldTitleCentered
            {
                get
                {
                    if (boldTitleCentered == null)
                    {
                        boldTitleCentered = new GUIStyle(BoldTitle) {alignment = TextAnchor.MiddleCenter};
                    }

                    return boldTitleCentered;
                }
            }

            public static GUIStyle BoldTitleRight
            {
                get
                {
                    if (boldTitleRight == null)
                    {
                        boldTitleRight = new GUIStyle(BoldTitle) {alignment = TextAnchor.MiddleRight};
                    }

                    return boldTitleRight;
                }
            }

            public static GUIStyle Button
            {
                get
                {
                    if (button == null)
                    {
                        button = new GUIStyle(nameof(Button));
                    }

                    return button;
                }
            }

            public static GUIStyle ButtonLeft
            {
                get
                {
                    if (buttonLeft == null)
                    {
                        buttonLeft = new GUIStyle(nameof(ButtonLeft));
                    }

                    return buttonLeft;
                }
            }

            public static GUIStyle ButtonLeftSelected
            {
                get
                {
                    if (buttonLeftSelected == null)
                    {
                        buttonLeftSelected = new GUIStyle(ButtonLeft) {normal = new GUIStyle(ButtonLeft).onNormal};
                    }

                    return buttonLeftSelected;
                }
            }

            public static GUIStyle ButtonMid
            {
                get
                {
                    if (buttonMid == null)
                    {
                        buttonMid = new GUIStyle(nameof(ButtonMid));
                    }

                    return buttonMid;
                }
            }

            public static GUIStyle ButtonMidSelected
            {
                get
                {
                    if (buttonMidSelected == null)
                    {
                        buttonMidSelected = new GUIStyle(ButtonMid) {normal = new GUIStyle(ButtonMid).onNormal};
                    }

                    return buttonMidSelected;
                }
            }

            public static GUIStyle ButtonRight
            {
                get
                {
                    if (buttonRight == null)
                    {
                        buttonRight = new GUIStyle(nameof(ButtonRight));
                    }

                    return buttonRight;
                }
            }

            public static GUIStyle ButtonRightSelected
            {
                get
                {
                    if (buttonRightSelected == null)
                    {
                        buttonRightSelected = new GUIStyle(ButtonRight) {normal = new GUIStyle(ButtonRight).onNormal};
                    }

                    return buttonRightSelected;
                }
            }

            public static GUIStyle ButtonSelected
            {
                get
                {
                    if (buttonSelected == null)
                    {
                        buttonSelected = new GUIStyle(Button) {normal = new GUIStyle(Button).onNormal};
                    }

                    return buttonSelected;
                }
            }

            public static GUIStyle IconButton
            {
                get
                {
                    if (iconButton == null)
                    {
                        iconButton = new GUIStyle(GUIStyle.none) {padding = new RectOffset(1, 1, 1, 1)};
                    }

                    return iconButton;
                }
            }

            public static GUIStyle Label
            {
                get
                {
                    if (label == null)
                    {
                        label = new GUIStyle(EditorStyles.label) {margin = new RectOffset(0, 0, 0, 0)};
                    }

                    return label;
                }
            }

            public static GUIStyle LabelCentered
            {
                get
                {
                    if (labelCentered == null)
                    {
                        labelCentered = new GUIStyle(Label)
                        {
                            alignment = TextAnchor.MiddleCenter, margin = new RectOffset(0, 0, 0, 0)
                        };
                    }

                    return labelCentered;
                }
            }

            public static GUIStyle LeftAlignedGreyMiniLabel
            {
                get
                {
                    if (leftAlignedGreyMiniLabel == null)
                    {
                        leftAlignedGreyMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                        {
                            alignment = TextAnchor.MiddleLeft, clipping = TextClipping.Clip
                        };
                        if (UnityVersion.IsVersionOrGreater(2019, 3))
                        {
                            leftAlignedGreyMiniLabel.margin = new RectOffset(4, 4, 4, 4);
                        }
                    }

                    return leftAlignedGreyMiniLabel;
                }
            }

            public static GUIStyle MiniButton
            {
                get
                {
                    if (miniButton == null)
                    {
                        miniButton = new GUIStyle(EditorStyles.miniButton);
                    }

                    return miniButton;
                }
            }

            public static GUIStyle MiniButtonLeft
            {
                get
                {
                    if (miniButtonLeft == null)
                    {
                        miniButtonLeft = new GUIStyle(EditorStyles.miniButtonLeft);
                    }

                    return miniButtonLeft;
                }
            }

            public static GUIStyle MiniButtonLeftSelected
            {
                get
                {
                    if (miniButtonLeftSelected == null)
                    {
                        miniButtonLeftSelected = new GUIStyle(MiniButtonLeft)
                        {
                            normal = new GUIStyle(MiniButtonLeft).onNormal
                        };
                    }

                    return miniButtonLeftSelected;
                }
            }

            public static GUIStyle MiniButtonMid
            {
                get
                {
                    if (miniButtonMid == null)
                    {
                        miniButtonMid = new GUIStyle(EditorStyles.miniButtonMid);
                    }

                    return miniButtonMid;
                }
            }

            public static GUIStyle MiniButtonMidSelected
            {
                get
                {
                    if (miniButtonMidSelected == null)
                    {
                        miniButtonMidSelected = new GUIStyle(MiniButtonMid)
                        {
                            normal = new GUIStyle(MiniButtonMid).onNormal
                        };
                    }

                    return miniButtonMidSelected;
                }
            }

            public static GUIStyle MiniButtonRight
            {
                get
                {
                    if (miniButtonRight == null)
                    {
                        miniButtonRight = new GUIStyle(EditorStyles.miniButtonRight);
                    }

                    return miniButtonRight;
                }
            }

            public static GUIStyle MiniButtonRightSelected
            {
                get
                {
                    if (miniButtonRightSelected == null)
                    {
                        miniButtonRightSelected = new GUIStyle(MiniButtonRight)
                        {
                            normal = new GUIStyle(MiniButtonRight).onNormal
                        };
                    }

                    return miniButtonRightSelected;
                }
            }

            public static GUIStyle MiniButtonSelected
            {
                get
                {
                    if (miniButtonSelected == null)
                    {
                        miniButtonSelected = new GUIStyle(MiniButton) {normal = new GUIStyle(MiniButton).onNormal};
                    }

                    return miniButtonSelected;
                }
            }

            public static GUIStyle MultiLineLabel
            {
                get
                {
                    if (multiLineLabel == null)
                    {
                        multiLineLabel = new GUIStyle(EditorStyles.label)
                        {
                            richText = true, stretchWidth = false, wordWrap = true
                        };
                    }

                    return multiLineLabel;
                }
            }

            public static GUIStyle[] NodeBoxes
            {
                get
                {
                    if ((nodeBoxes == null) || (nodeBoxes.Length == 0))
                    {
                        nodeBoxes = new GUIStyle[] {"TE NodeBox", "TE NodeBoxSelected"};
                    }

                    return nodeBoxes;
                }
            }

            public static GUIStyle NodeLabelTop
            {
                get
                {
                    if (nodeLabelTop == null)
                    {
                        nodeLabelTop = "TE NodeLabelTop";
                    }

                    return nodeLabelTop;
                }
            }

            public static GUIStyle None
            {
                get
                {
                    if (none == null)
                    {
                        none = new GUIStyle
                        {
                            margin = new RectOffset(0, 0, 0, 0),
                            padding = new RectOffset(0, 0, 0, 0),
                            border = new RectOffset(0, 0, 0, 0)
                        };
                    }

                    return none;
                }
            }

            public static GUIStyle PinLabel
            {
                get
                {
                    if (pinLabel == null)
                    {
                        pinLabel = "TE PinLabel";
                    }

                    return pinLabel;
                }
            }

            public static GUIStyle RightAlignedGreyMiniLabel
            {
                get
                {
                    if (rightAlignedGreyMiniLabel == null)
                    {
                        rightAlignedGreyMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                        {
                            alignment = TextAnchor.MiddleRight,
                            clipping = TextClipping.Overflow,
                            border = new RectOffset(1, 1, 1, 1),
                            fontSize = 11
                        };
                        if (UnityVersion.IsVersionOrGreater(2019, 3))
                        {
                            rightAlignedGreyMiniLabel.margin = new RectOffset(4, 4, 4, 4);
                        }
                    }

                    return rightAlignedGreyMiniLabel;
                }
            }

            public static GUIStyle SectionHeader
            {
                get
                {
                    if (sectionHeader == null)
                    {
                        sectionHeader = new GUIStyle(EditorStyles.largeLabel)
                        {
                            fontSize = 22,
                            margin = new RectOffset(0, 0, 5, 0),
                            fontStyle = FontStyle.Normal,
                            wordWrap = true,
                            font = EditorStyles.centeredGreyMiniLabel.font,
                            overflow = new RectOffset(0, 0, 0, 0)
                        };
                    }

                    return sectionHeader;
                }
            }

            public static GUIStyle SmallTitle
            {
                get
                {
                    if (smallTitle == null)
                    {
                        smallTitle = new GUIStyle(Title)
                        {
                            font = UnityEngine.GUI.skin.button.font, fontSize = 10, fixedHeight = 16f
                        };

                        var textColor = smallTitle.normal.textColor;
                        textColor.a *= 0.7f;
                        smallTitle.normal.textColor = textColor;
                    }

                    return smallTitle;
                }
            }

            public static GUIStyle SmallTitleCentered
            {
                get
                {
                    if (smallTitleCentered == null)
                    {
                        smallTitleCentered = new GUIStyle(SmallTitle) {alignment = TextAnchor.MiddleCenter};
                    }

                    return smallTitleCentered;
                }
            }

            public static GUIStyle SmallTitleRight
            {
                get
                {
                    if (smallTitleRight == null)
                    {
                        smallTitleRight = new GUIStyle(SmallTitle) {alignment = TextAnchor.MiddleRight};
                    }

                    return smallTitleRight;
                }
            }

            public static GUIStyle Subtitle
            {
                get
                {
                    if (subtitle == null)
                    {
                        subtitle = new GUIStyle(Title)
                        {
                            font = UnityEngine.GUI.skin.button.font,
                            fontSize = 10,
                            contentOffset = new Vector2(0.0f, -3f),
                            fixedHeight = 16f
                        };
                        var textColor = subtitle.normal.textColor;
                        textColor.a *= 0.7f;
                        subtitle.normal.textColor = textColor;
                    }

                    return subtitle;
                }
            }

            public static GUIStyle SubtitleCentered
            {
                get
                {
                    if (subtitleCentered == null)
                    {
                        subtitleCentered = new GUIStyle(Subtitle) {alignment = TextAnchor.MiddleCenter};
                    }

                    return subtitleCentered;
                }
            }

            public static GUIStyle SubtitleRight
            {
                get
                {
                    if (subtitleRight == null)
                    {
                        subtitleRight = new GUIStyle(Subtitle) {alignment = TextAnchor.MiddleRight};
                    }

                    return subtitleRight;
                }
            }

            public static GUIStyle Title
            {
                get
                {
                    if (title == null)
                    {
                        title = new GUIStyle(EditorStyles.label);
                    }

                    return title;
                }
            }

            public static GUIStyle TitleCentered
            {
                get
                {
                    if (titleCentered == null)
                    {
                        titleCentered = new GUIStyle(Title) {alignment = TextAnchor.MiddleCenter};
                    }

                    return titleCentered;
                }
            }

            public static GUIStyle TitleRight
            {
                get
                {
                    if (titleRight == null)
                    {
                        titleRight = new GUIStyle(Title) {alignment = TextAnchor.MiddleRight};
                    }

                    return titleRight;
                }
            }
            
            
            public static GUIStyle TreeLabel
            {
                get
                {
                    if (treeLabel == null)
                    {
                        treeLabel = new GUIStyle(EditorStyles.label)
                        {
                            //margin = new RectOffset(0, 0, 0, 0),
                        };
                        
                        treeLabel.normal.background = Texture2D.whiteTexture;
                        treeLabel.border = new RectOffset(1, 1, 1, 1);
                        
                    }

                    return treeLabel;
                }
            }
        }
    }
}
