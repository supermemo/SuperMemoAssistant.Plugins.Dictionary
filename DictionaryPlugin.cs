#region License & Metadata

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




using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using mshtml;
using SuperMemoAssistant.Extensions;
using SuperMemoAssistant.Plugins.Dictionary.Interop;
using SuperMemoAssistant.Plugins.Dictionary.UI;
using SuperMemoAssistant.Services;
using SuperMemoAssistant.Services.Sentry;
using SuperMemoAssistant.Sys.IO.Devices;

namespace SuperMemoAssistant.Plugins.Dictionary
{
  // ReSharper disable once ClassNeverInstantiated.Global
  public class DictionaryPlugin : SentrySMAPluginBase<DictionaryPlugin>
  {
    #region Properties & Fields - Non-Public

    private DictionaryService _dictionaryService;

    private SynchronizationContext _syncContext;

    #endregion




    #region Properties & Fields - Public

    public DictCfg Config { get; private set; }

    #endregion




    #region Properties Impl - Public

    /// <inheritdoc />
    public override string Name => "Dictionary";

    #endregion




    #region Methods Impl

    /// <inheritdoc />
    protected override void PluginInit()
    {
      _syncContext = new DispatcherSynchronizationContext();
      SynchronizationContext.SetSynchronizationContext(_syncContext);

      Config = Svc.Configuration.Load<DictCfg>().Result ?? new DictCfg();
      //SettingsModels = new List<INotifyPropertyChangedEx> { Config };

      _dictionaryService = new DictionaryService();

      Svc.KeyboardHotKey.RegisterHotKey(
        new HotKey(true,
                   true,
                   false,
                   false,
                   Key.D,
                   "Dictionary: Lookup word"),
        LookupWord);

      PublishService<IDictionaryService, DictionaryService>(_dictionaryService);
    }

    #endregion




    #region Methods

    public void LookupWord()
    {
      var ctrlGroup = Svc.SMA.UI.ElementWindow.ControlGroup;
      var htmlCtrl  = ctrlGroup?.FocusedControl.AsHtml();
      var htmlDoc   = htmlCtrl?.Document;
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

      _syncContext.Post(
        o =>
        {
          var wdw = new DictionaryWindow(_dictionaryService,
                                         text);
          wdw.Show();
          wdw.Activate();
        },
        null
      );
    }

    #endregion




    //public override void SettingsSaved(object cfgObject)
    //{
    //  Svc.Configuration.Save<DictCfg>(Config).Wait();
    //}
  }
}
