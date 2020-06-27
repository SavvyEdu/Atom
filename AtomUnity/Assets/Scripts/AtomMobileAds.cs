using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

/// <summary>
/// https://developers.google.com/admob/unity/interstitial
/// </summary>
public class AtomMobileAds : MonoBehaviour
{
#if UNITY_IOS || UNITY_ANDROID

    private InterstitialAd interstitial;
    void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });

        RequestInterstitial();
    }

    private void RequestInterstitial()
    {
        //TEST: ca-app-pub-3940256099942544/1033173712
        //REAL ANDROID: ca-app-pub-1691376898912539/1533815312
        //REAL IOS ca-app-pub-3940256099942544/4411468910

#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-1691376898912539/1533815312";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";*/
#else
        string adUnitId = "unexpected_platform";
#endif
        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);

        // Create an empty ad request.

        RequestConfiguration requestConfiguration = new RequestConfiguration.Builder()
            .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.True)
            .SetTagForUnderAgeOfConsent(TagForUnderAgeOfConsent.True)
            .SetMaxAdContentRating(MaxAdContentRating.G)
            .build();

        MobileAds.SetRequestConfiguration(requestConfiguration);
        
        AdRequest request = new AdRequest
            .Builder()
            .TagForChildDirectedTreatment(true)
            .Build();
        
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

    public void ShowInterstitial()
    {
        if (this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
        }
    }

    private void OnDestroy()
    {
        interstitial.Destroy();
    }

#endif

}
