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
// Created On:   2019/02/22 23:25
// Modified On:  2019/02/23 14:40
// Modified By:  Alexis

#endregion




using System.Collections.Generic;
using SuperMemoAssistant.Interop.SuperMemo.Elements.Types;
using SuperMemoAssistant.Plugins.Dictionary.Interop;
using SuperMemoAssistant.Plugins.Dictionary.Interop.OxfordDictionaries.Models;
using SuperMemoAssistant.Plugins.Dictionary.OxfordDictionaries;
using SuperMemoAssistant.Services;
using SuperMemoAssistant.Sys;
using SuperMemoAssistant.Sys.Remoting;

namespace SuperMemoAssistant.Plugins.Dictionary
{
  internal class DictionaryService : SMMarshalByRefObject, IDictionaryService
  {
    #region Properties & Fields - Non-Public

    private DictCfg                Config           => Svc<DictionaryPlugin>.Plugin.Config;
    private OxfordDictionaryClient OxfordDictClient { get; set; }

    #endregion




    #region Constructors

    /// <inheritdoc />
    public DictionaryService()
    {
      OxfordDictClient = new OxfordDictionaryClient();
    }

    #endregion




    #region Properties Impl - Public

    /// <inheritdoc />
    public bool CredentialsAvailable =>
      string.IsNullOrWhiteSpace(Config.AppKey) == false
      && string.IsNullOrWhiteSpace(Config.AppId) == false;
    /// <inheritdoc />
    public IElement RootElement => Config.RootDictElement ?? Svc.SMA.Registry.Element.Root;

    #endregion




    #region Methods Impl

    /// <inheritdoc />
    public RemoteTask<EntryResult> LookupEntry(RemoteCancellationToken ct,
                                               string                  word,
                                               string                  language = "en")
    {
      OxfordDictClient.SetAuthentication(Config.AppId,
                                         Config.AppKey);

      return OxfordDictClient.LookupEntry(ct.Token(),
                                          word,
                                          language);
    }

    /// <inheritdoc />
    public RemoteTask<LemmatronResult> LookupLemma(RemoteCancellationToken ct,
                                                   string                  word,
                                                   string                  language = "en")
    {
      OxfordDictClient.SetAuthentication(Config.AppId,
                                         Config.AppKey);

      return OxfordDictClient.LookupLemma(ct.Token(),
                                          word,
                                          language);
    }

    /// <inheritdoc />
    public RemoteTask<List<OxfordDictionary>> GetAvailableDictionaries(RemoteCancellationToken ct)
    {
      OxfordDictClient.SetAuthentication(Config.AppId,
                                         Config.AppKey);

      return OxfordDictClient.GetAvailableDictionaries(ct.Token());
    }

    #endregion
  }
}
