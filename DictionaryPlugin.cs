﻿#region License & Metadata

// The MIT License (MIT)
// 
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// 
// 
// Created On:   2018/12/30 19:48
// Modified On:  2019/02/25 17:45
// Modified By:  Alexis

#endregion




using System.Windows;
using System.Windows.Input;
using Anotar.Serilog;
using mshtml;
using SuperMemoAssistant.Extensions;
using SuperMemoAssistant.Plugins.Dictionary.Interop;
using SuperMemoAssistant.Plugins.Dictionary.UI;
using SuperMemoAssistant.Services;
using SuperMemoAssistant.Services.IO.HotKeys;
using SuperMemoAssistant.Services.IO.Keyboard;
using SuperMemoAssistant.Services.Sentry;
using SuperMemoAssistant.Services.UI.Configuration;
using SuperMemoAssistant.Sys.IO.Devices;

namespace SuperMemoAssistant.Plugins.Dictionary
{
  // ReSharper disable once ClassNeverInstantiated.Global
  public class DictionaryPlugin : SentrySMAPluginBase<DictionaryPlugin>
  {
    #region Properties & Fields - Non-Public

    private DictionaryService _dictionaryService;

    #endregion




    #region Properties & Fields - Public

    public DictCfg Config { get; private set; }

    #endregion




    #region Properties Impl - Public

    /// <inheritdoc />
    public override string Name => "Dictionary";
    
    public override bool HasSettings => true;

    #endregion




    #region Methods Impl

    /// <inheritdoc />
    protected override void PluginInit()
    {
      Config = Svc.Configuration.Load<DictCfg>().Result ?? new DictCfg();

      _dictionaryService = new DictionaryService();

      Svc.HotKeyManager.RegisterGlobal(
        "LookupWord",
        "Dictionary: Lookup word",
        HotKeyScope.SMBrowser,
        new HotKey(Key.D, KeyModifiers.CtrlAlt),
        LookupWord
      );

      PublishService<IDictionaryService, DictionaryService>(_dictionaryService);
    }
    
    /// <inheritdoc />
    public override void ShowSettings()
    {
      Application.Current.Dispatcher.Invoke(
        () => new ConfigurationWindow(HotKeyManager.Instance, Config).ShowAndActivate()
      );
    }

    #endregion




    #region Methods
    
    [LogToErrorOnException]
    public void LookupWord()
    {
      var ctrlGroup = Svc.SM.UI.ElementWdw.ControlGroup;
      var htmlCtrl  = ctrlGroup?.FocusedControl.AsHtml();
      var htmlDoc   = htmlCtrl?.GetDocument();
      var sel       = htmlDoc?.selection;

      if (!(sel?.createRange() is IHTMLTxtRange textSel))
        return;

      var text = textSel.text?.Trim(' ',
                                    '\t',
                                    '\r',
                                    '\n');

      if (string.IsNullOrWhiteSpace(text))
        return;

      /*
      int spaceIdx = text.IndexOfAny(new[] { ' ', '\r' });

      if (spaceIdx > 0)
        text = text.Substring(0,
                              spaceIdx);

      if (spaceIdx == 0 || string.IsNullOrWhiteSpace(text))
        return;
      */

      Application.Current.Dispatcher.Invoke(
        () =>
        {
          var wdw = new DictionaryWindow(_dictionaryService, text);
          wdw.ShowAndActivate();
        }
      );
    }

    #endregion
  }
}
