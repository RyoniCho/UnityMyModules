//
//  UnitySwiftPluginBridge.mm
//  MapleM_iOS_SwiftPlugin
//
//  Created by 조을연 on 2022/07/19.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "UnitySwiftPluginBridge.h"
#include "UnityFramework/UnityFramework-Swift.h"
#include "UnityAppController.h"

static NSString* const NSStringFromCString(const char* string)
{
    if(string !=NULL)
    {
        return [NSString stringWithUTF8String:string];
        
    }
    else
        nil;
    
}

extern "C"
{
    void showToast(const char* message)
    {
        [[UnitySwiftPlugin shared] ShowToastWithMessage:NSStringFromCString(message)];
    }

    void requestMicrophonePermission()
    {
        [[UnitySwiftPlugin shared] RequestMicrophonePermission];
    }

    bool canShowRequestMicPermission()
    {
        return  [[UnitySwiftPlugin shared] CanShowRequestMicPermission];
    }

    bool checkCameraPermissionAllowed()
    {
        return  [[UnitySwiftPlugin shared] CheckCameraPermissionAllowed];
    }

    bool checkPhotoLibraryPermissionAllowed()
    {
        return  [[UnitySwiftPlugin shared] CheckPhotoLibraryPermissionAllowed];
    }

    void showAlertPopup(const char* title, const char* message,bool setCancelButton, DelegateCallbackFunction okCallback, DelegateCallbackFunction cancelCallback)
    {
        [[UnitySwiftPlugin shared] ShowNativeAlertPopupWithTitle:NSStringFromCString(title) message:NSStringFromCString(message) setCancelButton:setCancelButton okCallback:okCallback cancelCallback:cancelCallback];
    }
}
