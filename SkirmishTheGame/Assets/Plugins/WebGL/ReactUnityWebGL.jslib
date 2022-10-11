mergeInto(LibraryManager.library, {


  CheckBalanceERC20: function () {
    dispatchReactUnityEvent("CheckBalanceERC20");
  },

  ConnectWallet: function () {
    dispatchReactUnityEvent("ConnectWallet");
  },

  CallAddressUpdate: function () {
    dispatchReactUnityEvent("CallAddressUpdate");
  },

   RefreshWebsite: function () {
    window.location.reload();
  },






    RequestSquadData: function (squadData) {
    dispatchReactUnityEvent("RequestSquadData", UTF8ToString(squadData));
  },


      SendSquadData: function (squadData) {
    dispatchReactUnityEvent("SendSquadData", UTF8ToString(squadData));
  },

     SetSNS: function (SNS) {
    dispatchReactUnityEvent("SetSNS", UTF8ToString(SNS));
  },


     CheckSNS: function (SNS) {
    dispatchReactUnityEvent("CheckSNS", UTF8ToString(SNS));
  },



});

