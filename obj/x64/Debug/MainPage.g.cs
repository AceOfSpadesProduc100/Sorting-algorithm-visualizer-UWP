﻿#pragma checksum "C:\Users\CDog2\source\repos\AlgoUWP\MainPage.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "1A660119252E2CA3212B0854E39534C3BC89730FF1F7B458BBEDEF5C1738C3B6"
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
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.19041.685")]
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
                }
                break;
            case 2: // MainPage.xaml line 14
                {
                    this.canv = (global::Windows.UI.Xaml.Controls.Canvas)(target);
                    ((global::Windows.UI.Xaml.Controls.Canvas)this.canv).PointerPressed += this.Canv_MouseLeftButtonDown;
                    ((global::Windows.UI.Xaml.Controls.Canvas)this.canv).PointerMoved += this.Canv_MouseMove;
                }
                break;
            case 3: // MainPage.xaml line 19
                {
                    this.visualizeBtn = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.visualizeBtn).Click += this.Visualize_Click;
                }
                break;
            case 4: // MainPage.xaml line 20
                {
                    this.shuffleBtn = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.shuffleBtn).Click += this.ShuffleBtn_Click;
                }
                break;
            case 5: // MainPage.xaml line 21
                {
                    this.comboBox = (global::Windows.UI.Xaml.Controls.ComboBox)(target);
                }
                break;
            case 6: // MainPage.xaml line 36
                {
                    this.distribcomboBox = (global::Windows.UI.Xaml.Controls.ComboBox)(target);
                }
                break;
            case 7: // MainPage.xaml line 61
                {
                    this.shufflecomboBox = (global::Windows.UI.Xaml.Controls.ComboBox)(target);
                }
                break;
            case 8: // MainPage.xaml line 108
                {
                    this.speedSlider = (global::Windows.UI.Xaml.Controls.Slider)(target);
                }
                break;
            case 9: // MainPage.xaml line 109
                {
                    this.pauseButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.pauseButton).Click += this.PauseButton_Click;
                }
                break;
            case 10: // MainPage.xaml line 110
                {
                    this.ArraySize = (global::Microsoft.UI.Xaml.Controls.NumberBox)(target);
                }
                break;
            case 11: // MainPage.xaml line 113
                {
                    this.CompsText = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 12: // MainPage.xaml line 114
                {
                    this.SwapsText = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 13: // MainPage.xaml line 115
                {
                    this.ReversalsText = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 14: // MainPage.xaml line 116
                {
                    this.WritesText = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
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
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.19041.685")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

