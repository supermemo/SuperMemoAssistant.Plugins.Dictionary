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
// Created On:   2019/03/02 18:29
// Modified On:  2019/03/28 15:23
// Modified By:  Alexis

#endregion




using System.Collections.Generic;
using System.ComponentModel;
using Forge.Forms.Annotations;
using Newtonsoft.Json;
using SuperMemoAssistant.Interop.SuperMemo.Elements.Types;
using SuperMemoAssistant.Plugins.Dictionary.Interop;
using SuperMemoAssistant.Services;
using SuperMemoAssistant.Services.UI.Configuration.ElementPicker;
using SuperMemoAssistant.Sys.ComponentModel;

namespace SuperMemoAssistant.Plugins.Dictionary
{
  [Form(Mode = DefaultFields.None)]
  [Title("Dictionary Settings",
    IsVisible = "{Env DialogHostContext}")]
  [DialogAction("cancel",
    "Cancel",
    IsCancel = true)]
  [DialogAction("save",
    "Save",
    IsDefault = true,
    Validates = true)]
  public class DictCfg : IElementPickerCallback, INotifyPropertyChangedEx
  {
    #region Properties & Fields - Public

    [Field(Name                                    = "Layout")]
    [SelectFrom("{Binding Layouts}", SelectionType = SelectionType.ComboBox)]
    public string Layout { get; set; }

    [Field(Name = "Default Extract Priority (%)")]
    [Value(Must.BeGreaterThanOrEqualTo,
      0,
      StrictValidation = true)]
    [Value(Must.BeLessThanOrEqualTo,
      100,
      StrictValidation = true)]
    public double ExtractPriority { get; set; } = DictionaryConst.DefaultExtractPriority;

    [Field(Name = "Oxford Dict. App Id")]
    public string AppId { get; set; }
    [Field(Name = "Oxford Dict. App Key")]
    public string AppKey { get; set; }

    [JsonIgnore]
    [Action(ElementPicker.ElementPickerAction,
      "Browse",
      Placement = Placement.Inline)]
    [Field(Name  = "Root Element",
      IsReadOnly = true)]
    public string ElementField
    {
      // ReSharper disable once ValueParameterNotUsed
      set
      {
        /* empty */
      }
      get => RootDictElement == null
        ? "N/A"
        : RootDictElement.ToString();
    }


    //
    // Config only

    public int RootDictElementId { get; set; }

    public string OxfordLanguagesJson { get; set; } = DictionaryConst.OxfordLanguagesJson;


    //
    // Helpers

    [JsonIgnore]
    public IEnumerable<string> Layouts => Svc.SMA.Layouts;

    [JsonIgnore]
    public IElement RootDictElement => Svc.SM.Registry.Element[RootDictElementId <= 0 ? 1 : RootDictElementId];

    #endregion




    #region Properties Impl - Public

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    [JsonIgnore]
    public bool IsChanged { get; set; }

    #endregion




    #region Methods Impl

    public override string ToString()
    {
      return "Dictionary";
    }

    public void SetElement(IElement elem)
    {
      RootDictElementId = elem.Id;

      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ElementField)));
    }

    #endregion




    #region Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion
  }
}
