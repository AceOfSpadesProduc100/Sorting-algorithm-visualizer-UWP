﻿#pragma checksum "C:\Users\CDog2\OneDrive\AlgoUWP\MainPage.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "2F4F73CA83DA4676D3EC396CD4845CC09C88720AD862CF28340CC19A75779C4B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AlgoUWP
{
    partial class MainPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 0.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1: // MainPage.xaml line 1
                {
                    global::Windows.UI.Xaml.Controls.Page element1 = (global::Windows.UI.Xaml.Controls.Page)(target);
                    ((global::Windows.UI.Xaml.Controls.Page)element1).SizeChanged += this.Window_SizeChanged;
                    ((global::Windows.UI.Xaml.Controls.Page)element1).PointerReleased += this.Window_MouseLeftButtonUp;
                    ((global::Windows.UI.Xaml.Controls.Page)element1).Loaded += this.Page_Loaded;
                }
                break;
            case 2: // MainPage.xaml line 18
                {
                    this.___No_Name_ = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 3: // MainPage.xaml line 20
                {
                    this.canv = (global::Windows.UI.Xaml.Controls.Canvas)(target);
                    ((global::Windows.UI.Xaml.Controls.Canvas)this.canv).PointerPressed += this.Canv_MouseLeftButtonDown;
                    ((global::Windows.UI.Xaml.Controls.Canvas)this.canv).PointerMoved += this.Canv_MouseMove;
                }
                break;
            case 4: // MainPage.xaml line 25
                {
                    this.visualizeBtn = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.visualizeBtn).Click += this.Visualize_Click;
                }
                break;
            case 5: // MainPage.xaml line 26
                {
                    this.shuffleBtn = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.shuffleBtn).Click += this.ShuffleBtn_Click;
                }
                break;
            case 6: // MainPage.xaml line 27
                {
                    this.comboBox = (global::Windows.UI.Xaml.Controls.Button)(target);
                }
                break;
            case 7: // MainPage.xaml line 60
                {
                    this.distribcomboBox = (global::Windows.UI.Xaml.Controls.ComboBox)(target);
                }
                break;
            case 8: // MainPage.xaml line 62
                {
                    this.shufflecomboBox = (global::Windows.UI.Xaml.Controls.ComboBox)(target);
                }
                break;
            case 9: // MainPage.xaml line 64
                {
                    this.speedSlider = (global::Windows.UI.Xaml.Controls.Slider)(target);
                }
                break;
            case 10: // MainPage.xaml line 65
                {
                    this.pauseButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.pauseButton).Click += this.PauseButton_Click;
                }
                break;
            case 11: // MainPage.xaml line 66
                {
                    this.ArraySize = (global::Microsoft.UI.Xaml.Controls.NumberBox)(target);
                    ((global::Microsoft.UI.Xaml.Controls.NumberBox)this.ArraySize).ValueChanged += this.ArraySize_ValueChanged;
                }
                break;
            case 12: // MainPage.xaml line 70
                {
                    this.ReadsText = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 13: // MainPage.xaml line 71
                {
                    this.CompsText = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 14: // MainPage.xaml line 72
                {
                    this.SwapsText = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 15: // MainPage.xaml line 73
                {
                    this.ReversalsText = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 16: // MainPage.xaml line 74
                {
                    this.MainWritesText = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 17: // MainPage.xaml line 75
                {
                    this.AuxWritesText = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 18: // MainPage.xaml line 76
                {
                    this.BucketCountBox = (global::Microsoft.UI.Xaml.Controls.NumberBox)(target);
                }
                break;
            case 19: // MainPage.xaml line 77
                {
                    this.ReadsText_Copy = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 20: // MainPage.xaml line 78
                {
                    this.CompsText_Copy = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 21: // MainPage.xaml line 79
                {
                    this.SwapsText_Copy = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 22: // MainPage.xaml line 80
                {
                    this.ReversalsText_Copy = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 23: // MainPage.xaml line 81
                {
                    this.MainWritesText_Copy = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 24: // MainPage.xaml line 82
                {
                    this.AuxWritesText_Copy = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 25: // MainPage.xaml line 83
                {
                    this.ShuffleCheck = (global::Windows.UI.Xaml.Controls.CheckBox)(target);
                }
                break;
            case 26: // MainPage.xaml line 51
                {
                    global::Windows.UI.Xaml.Controls.MenuFlyoutItem element26 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target);
                    ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)element26).Click += this.MenuFlyoutItem_Click;
                }
                break;
            case 27: // MainPage.xaml line 52
                {
                    global::Windows.UI.Xaml.Controls.MenuFlyoutItem element27 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target);
                    ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)element27).Click += this.MenuFlyoutItem_Click;
                }
                break;
            case 28: // MainPage.xaml line 53
                {
                    global::Windows.UI.Xaml.Controls.MenuFlyoutItem element28 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target);
                    ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)element28).Click += this.MenuFlyoutItem_Click;
                }
                break;
            case 29: // MainPage.xaml line 45
                {
                    global::Windows.UI.Xaml.Controls.MenuFlyoutItem element29 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target);
                    ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)element29).Click += this.MenuFlyoutItem_Click;
                }
                break;
            case 30: // MainPage.xaml line 46
                {
                    global::Windows.UI.Xaml.Controls.MenuFlyoutItem element30 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target);
                    ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)element30).Click += this.MenuFlyoutItem_Click;
                }
                break;
            case 31: // MainPage.xaml line 47
                {
                    global::Windows.UI.Xaml.Controls.MenuFlyoutItem element31 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target);
                    ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)element31).Click += this.MenuFlyoutItem_Click;
                }
                break;
            case 32: // MainPage.xaml line 48
                {
                    global::Windows.UI.Xaml.Controls.MenuFlyoutItem element32 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target);
                    ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)element32).Click += this.MenuFlyoutItem_Click;
                }
                break;
            case 33: // MainPage.xaml line 41
                {
                    global::Windows.UI.Xaml.Controls.MenuFlyoutItem element33 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target);
                    ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)element33).Click += this.MenuFlyoutItem_Click;
                }
                break;
            case 34: // MainPage.xaml line 42
                {
                    global::Windows.UI.Xaml.Controls.MenuFlyoutItem element34 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target);
                    ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)element34).Click += this.MenuFlyoutItem_Click;
                }
                break;
            case 35: // MainPage.xaml line 35
                {
                    global::Windows.UI.Xaml.Controls.MenuFlyoutItem element35 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target);
                    ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)element35).Click += this.MenuFlyoutItem_Click;
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 0.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

