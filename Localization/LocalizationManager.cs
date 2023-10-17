using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class LocalizationManager : SingletonStatic<LocalizationManager>
{

    /// <summary>
    /// 테이블 이름과, 키값을 통해 텍스트를 현지화된 텍스트로 변경합니다.
    /// </summary>
    public string GetLocalizedString(string tableRef, string key)
    {
        var localizedString = new LocalizedString(tableRef, key);
        return localizedString.GetLocalizedString();
    }

    public string GetLocalizedString(LocalizedString local)
    {
        return local.GetLocalizedString();
    }

    public string GetLocalizedStringValue(string tableRef, string key, string value)
    {
        var localizedString = new LocalizedString(tableRef, key);
        return localizedString.GetLocalizedString().Replace("@", value);
    }

    public string GetLocalizedStringValue(LocalizedString local, string value)
    {
        return local.GetLocalizedString().Replace("@", value);
    }
}
