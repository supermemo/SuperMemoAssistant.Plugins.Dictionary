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
// Modified On:  2019/01/03 03:10
// Modified By:  Alexis

#endregion




using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using mshtml;
using SuperMemoAssistant.Extensions;
using SuperMemoAssistant.Interop.Plugins;
using SuperMemoAssistant.Interop.SuperMemo.Elements.Types;
using SuperMemoAssistant.Plugins.Dictionary.Interop;
using SuperMemoAssistant.Plugins.Dictionary.Interop.OxfordDictionaries.Models;
using SuperMemoAssistant.Plugins.Dictionary.OxfordDictionaries;
using SuperMemoAssistant.Plugins.Dictionary.UI;
using SuperMemoAssistant.Services;
using SuperMemoAssistant.Sys.ComponentModel;
using SuperMemoAssistant.Sys.IO.Devices;

namespace SuperMemoAssistant.Plugins.Dictionary
{
  public class DictionaryPlugin : SMAPluginBase<DictionaryPlugin>, IDictionaryPlugin
  {
    #region Properties & Fields - Non-Public

    protected DictCfg Config { get; set; }

    private OxfordDictionaryClient OxfordDictClient { get; set; }


    protected SynchronizationContext SyncContext { get; set; }

    #endregion




    #region Properties Impl - Public

    /// <inheritdoc />
    public override string Name => "Dictionary";

    /// <inheritdoc />
    public bool CredentialsAvailable =>
      string.IsNullOrWhiteSpace(Config.AppKey) == false
      && string.IsNullOrWhiteSpace(Config.AppId) == false;

    /// <inheritdoc />
    public IElement RootElement => Config.RootDictElement ?? Svc.SMA.Registry.Element.Root;

    #endregion




    #region Methods Impl

    /// <inheritdoc />
    protected override void OnInit()
    {
      SyncContext = new DispatcherSynchronizationContext();
      SynchronizationContext.SetSynchronizationContext(SyncContext);

      OxfordDictClient = new OxfordDictionaryClient();
      Config           = Svc<DictionaryPlugin>.Configuration.Load<DictCfg>().Result ?? new DictCfg();
      SettingsModels   = new List<INotifyPropertyChangedEx> { Config };

      Svc.KeyboardHotKey.RegisterHotKey(
        new HotKey(true,
                   true,
                   false,
                   false,
                   Key.D,
                   "Dictionary: Lookup word"),
        LookupWord);

      Container.ComposeExportedValue<IDictionaryPlugin>(this);
    }

    /// <inheritdoc />
    public override void SettingsSaved(object cfgObject)
    {
      Svc<DictionaryPlugin>.Configuration.Save<DictCfg>(Config).Wait();
    }

    /// <inheritdoc />
    public Task<EntryResult> LookupEntry(CancellationToken ct,
                                         string            word,
                                         string            language = "en")
    {
      OxfordDictClient.SetAuthentication(Config.AppId,
                                         Config.AppKey);

      return OxfordDictClient.LookupEntry(ct,
                                          word,
                                          language);
    }

    /// <inheritdoc />
    public Task<LemmatronResult> LookupLemma(CancellationToken ct,
                                             string            word,
                                             string            language = "en")
    {
      OxfordDictClient.SetAuthentication(Config.AppId,
                                         Config.AppKey);

      return OxfordDictClient.LookupLemma(ct,
                                          word,
                                          language);
    }

    /// <inheritdoc />
    public Task<List<OxfordDictionary>> GetAvailableDictionaries(CancellationToken ct)
    {
      OxfordDictClient.SetAuthentication(Config.AppId,
                                         Config.AppKey);

      return OxfordDictClient.GetAvailableDictionaries(ct);
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

      SyncContext.Post(
        o =>
        {
          var wdw = new DictionaryWindow(this,
                               text);
          wdw.Show();
          wdw.Activate();
        },
        null
      );
    }

    #endregion
  }
}
