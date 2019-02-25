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
// Created On:   2018/12/31 00:20
// Modified On:  2019/02/23 23:19
// Modified By:  Alexis

#endregion




using System.ComponentModel;
using Forge.Forms.Annotations;
using Newtonsoft.Json;
using SuperMemoAssistant.Interop.SuperMemo.Elements.Types;
using SuperMemoAssistant.Services;
using SuperMemoAssistant.Sys.ComponentModel;
using SuperMemoAssistant.UI;

namespace SuperMemoAssistant.Plugins.Dictionary
{
  [Form(Mode = DefaultFields.None)]
  public class DictCfg : IElementPickerCallback, INotifyPropertyChangedEx
  {
    #region Properties & Fields - Public

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
    [JsonIgnore]
    public IElement RootDictElement => Svc.SMA.Registry.Element[RootDictElementId <= 0 ? 1 : RootDictElementId];
    public int RootDictElementId { get; set; }

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
    }

    #endregion




    #region Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion
  }
}
