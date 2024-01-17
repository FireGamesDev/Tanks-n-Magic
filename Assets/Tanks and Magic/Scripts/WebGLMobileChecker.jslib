var CheckMobile = {
    isMobileWebGL : function()
    {
        return Module.SystemInfo.mobile;
    }
};
mergeInto(LibraryManager.library, CheckMobile);