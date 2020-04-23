using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

namespace DemiumGames.AdMobManager
{
    public class AdMobManager : MonoBehaviour
    {

        public delegate void AdMobCallback();

        //Video Callbacks
        private AdMobCallback closeVideoCallback, rewardVideoCallback, rewardVideoOnLoad, rewardVideoOnFailedLoad;
        //Inter Callbacks
        private AdMobCallback interLoaded, interFailedToLoad, interClicked, interReturn;
        //Banner Callbacks
        private AdMobCallback bannerLoaded, bannerFailedToLoad, bannerClicked, bannerReturn;



		private string interId = "ca-app-pub-3340554034106910/9698522827";
		private string videoId = "ca-app-pub-3340554034106910/1807945585";
		private string bannerId = "ca-app-pub-3340554034106910/1755974693";

        private InterstitialAd inter;
        private RewardBasedVideoAd rewardVideo;
        private BannerView banner;
        private AdRequest interRequest, videoRequest, bannerRequest;

        private static AdMobManager instance;

        public static AdMobManager Instance
        {
            get { return instance; }
        }

        #region ChargeCoroutines
        IEnumerator ChargeVideo()
        {

            videoRequest = new AdRequest.Builder().Build();
            rewardVideo.LoadAd(videoRequest, videoId);


            yield return null;
        }

        IEnumerator ChargeInter()
        {
            inter = new InterstitialAd(interId);
            interRequest = new AdRequest.Builder().Build();
            inter.LoadAd(interRequest);

            inter.OnAdClosed += HandleOnInterReturn;
            inter.OnAdOpening += HandleOnInterClicked;
            inter.OnAdFailedToLoad += HandleOnInterFailedToLoad;
            inter.OnAdLoaded += HandleOnInterLoaded;

            yield return null;
        }

        IEnumerator ChargeBanner(AdSize bannerSize, AdPosition bannerPosition)
        {
            banner = new BannerView(bannerId, bannerSize, bannerPosition);
            bannerRequest = new AdRequest.Builder().Build();
            banner.LoadAd(bannerRequest);

            banner.OnAdClosed += HandleOnBannerReturn;
            banner.OnAdOpening += HandleOnBannerClicked;
            banner.OnAdFailedToLoad += HandleOnBannerFailedToLoad;
            banner.OnAdLoaded += HandleOnBannerLoaded;


            yield return null;
        }

        #endregion

        #region  Loaders

        public void LoadBanner(AdSize bannerSize, AdPosition bannerPosition)
        {
            StartCoroutine(ChargeBanner(bannerSize, bannerPosition));
        }

        public void LoadInter()
        {
            StartCoroutine(ChargeInter());
        }


        public void LoadVideo()
        {
            StartCoroutine(ChargeVideo());
        }

        #endregion

        #region Destroyers
        public void DestroyAllInstances()
        {

            DestroyInter();
            DestroyBanner();
        }

        public void DestroyInter()
        {
            if (inter != null)
            {
                inter.Destroy();
            }
        }

        public void DestroyBanner()
        {
            if (banner != null)
            {
                banner.Destroy();
            }
        }
        #endregion

        #region Showers
        public void ShowVideoAd(AdMobCallback closeCallback, AdMobCallback rewardCallback)
        {

            if (rewardVideo.IsLoaded())
            {
                this.closeVideoCallback = closeCallback;
                this.rewardVideoCallback = rewardCallback;
                rewardVideo.Show();
            }
        }

        public void ShowInter()
        {
            if (inter.IsLoaded())
            {
                inter.Show();
            }
        }

        public void ShowBanner()
        {
            banner.Show();
        }
        #endregion


        #region Hiders


        public void HideBanner()
        {
            banner.Hide();
        }
        #endregion

        #region Checkers
        public bool IsRewardedVideLoaded()
        {

            return rewardVideo != null && rewardVideo.IsLoaded();
        }


        public bool isInterLoaded()
        {
            return inter != null && inter.IsLoaded();
        }



        #endregion


        #region CallbackEvents


        public void SetOnBannerLoaded(AdMobCallback bannerLoaded)
        {
            this.bannerLoaded = bannerLoaded;
        }

        public void SetOnBannerFailedToLoad(AdMobCallback bannerFailedToLoad)
        {
            this.bannerFailedToLoad = bannerFailedToLoad;
        }

        public void SetOnBannerClicked(AdMobCallback bannerClicked)
        {
            this.bannerClicked = bannerClicked;
        }

        public void SetBannerReturn(AdMobCallback bannerReturn)
        {
            this.bannerReturn = bannerReturn;
        }



        public void SetBannerEvents(AdMobCallback bannerLoaded, AdMobCallback bannerFailedToLoad, AdMobCallback bannerClicked,
            AdMobCallback bannerReturn)
        {
            this.bannerLoaded = bannerLoaded;
            this.bannerFailedToLoad = bannerFailedToLoad;
            this.bannerClicked = bannerClicked;
            this.bannerReturn = bannerReturn;
        }


        public void SetOnInterLoaded(AdMobCallback interLoaded)
        {
            this.interLoaded = interLoaded;
        }

        public void SetOnInterFailedToLoad(AdMobCallback interFailedToLoad)
        {
            this.interFailedToLoad = interFailedToLoad;
        }

        public void SetOnInterClicked(AdMobCallback interClicked)
        {
            this.interClicked = interClicked;
        }

        public void SetOnInterReturn(AdMobCallback interReturn)
        {
            this.interReturn = interReturn;
        }
        public void SetInterEvents(AdMobCallback interLoaded, AdMobCallback interFailedToLoad, AdMobCallback interClicked,
            AdMobCallback interReturn)
        {
            this.interLoaded = interLoaded;
            this.interFailedToLoad = interFailedToLoad;
            this.interClicked = interClicked;
            this.interReturn = interReturn;
        }

        public void SetOnRewardLoaded(AdMobCallback rewardLoaded)
        {
            this.rewardVideoOnLoad = rewardLoaded;
        }

        public void SetOnRewardFailedToLoad(AdMobCallback rewardFailedToLoad)
        {
            this.rewardVideoOnFailedLoad = rewardFailedToLoad;
        }


        public void SetVideoEvents(AdMobCallback rewardLoaded, AdMobCallback rewardFailedLoad)
        {
            this.rewardVideoOnLoad = rewardLoaded;
            this.rewardVideoOnFailedLoad = rewardFailedLoad;
        }
        #endregion


        void Awake()
        {

            if (Instance == null)
            {
                Initialize();
            }
            else
            {
                Destroy(this.gameObject);
            }

        }

        private void Initialize(){
			instance = this;
			DontDestroyOnLoad(this.gameObject);
            InitializeRewardVideoInstance();
        }


        private void InitializeRewardVideoInstance(){
			rewardVideo = RewardBasedVideoAd.Instance;
			rewardVideo.OnAdLoaded += HandleOnRewardedVideoLoaded;
			rewardVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
			rewardVideo.OnAdClosed += RewardVideo_OnAdClosed;
        }



        #region Handlers
        public void HandleRewardBasedVideoRewarded(object sender, Reward args)
        {
            if (rewardVideoCallback != null)
                rewardVideoCallback();
        }

        private void HandleOnRewardedVideoLoaded(object obj, EventArgs args)
        {
            if (rewardVideoOnLoad != null)
                this.rewardVideoOnLoad();
        }

        private void HandleOnRewardedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            if (rewardVideoOnFailedLoad != null)
                this.rewardVideoOnFailedLoad();
        }

        private void RewardVideo_OnAdClosed(object sender, System.EventArgs e)
        {
            if (closeVideoCallback != null)
                closeVideoCallback();
        }
        private void HandleOnInterFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            if (interFailedToLoad != null)
                this.interFailedToLoad();
        }
        private void HandleOnBannerFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            if (bannerFailedToLoad != null)
                this.bannerFailedToLoad();
        }

        private void HandleOnBannerLoaded(object obj, EventArgs args)
        {
            if (bannerLoaded != null)
                this.bannerLoaded();
        }

        private void HandleOnInterLoaded(object obj, EventArgs args)
        {
            if (interLoaded != null)
                this.interLoaded();
        }

        private void HandleOnBannerClicked(object obj, EventArgs args)
        {
            if (bannerClicked != null)
                this.bannerClicked();
        }

        private void HandleOnInterClicked(object obj, EventArgs args)
        {
            if (interClicked != null)
                this.interClicked();
        }

        private void HandleOnInterReturn(object obj, EventArgs args)
        {
            if (interReturn != null)
                this.interReturn();
        }

        private void HandleOnBannerReturn(object obj, EventArgs args)
        {
            if (bannerReturn != null)
                this.bannerReturn();
        }



        #endregion
    }


}