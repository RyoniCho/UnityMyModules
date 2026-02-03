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

typedef struct {
    float x;
    float y;
} MousePosition;

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

    void showAlertPopup(int methodId,const char* title, const char* message,bool setCancelButton, DelegateCallbackFunction okCallback, DelegateCallbackFunction cancelCallback)
    {
        [[UnitySwiftPlugin shared] ShowNativeAlertPopupWithMethodId:methodId title:NSStringFromCString(title) message:NSStringFromCString(message) setCancelButton:setCancelButton okCallback:okCallback cancelCallback:cancelCallback];
    }
    
    void addObserverGCKeyboard(int methodId, DelegateBooleanCallbackFunction eventcallback)
    {
         [[UnitySwiftPlugin shared] AddObserverGCKeyboardWithMethodId:methodId eventCallback:eventcallback];
    }
    
    void addObserverGCController(int methodId, DelegateBooleanCallbackFunction eventcallback)
    {
         [[UnitySwiftPlugin shared] AddObserverGCControllerWithMethodId:methodId eventCallback:eventcallback];
    }
    
    void addObserverGCMouse(int methodId, DelegateBooleanCallbackFunction eventcallback)
    {
         [[UnitySwiftPlugin shared] AddObserverGCMouseWithMethodId:methodId eventCallback:eventcallback];
    }
    
    bool isAnyExternalKeyboardConnected()
    {
        return [[UnitySwiftPlugin shared] IsAnyExternalKeyboardConnected];
    }
    
    bool isAnyExternalMouseConnected()
    {
        return [[UnitySwiftPlugin shared] IsAnyExternalMouseConnected];
    }
    
    bool isAnyExternalGamepadConnected()
    {
        return [[UnitySwiftPlugin shared] IsAnyExternalGamepadConnected];
    }
    
    float getGCMouseScrollYAxisValue()
    {
        return [[UnitySwiftPlugin shared] GetGCMouseScrollYAxisValue];
    }
    
    float getGCMouseScrollXAxisValue()
    {
        return [[UnitySwiftPlugin shared] GetGCMouseScrollXAxisValue];
    }

    MousePosition GetMousePosition() {
        CGPoint point = [[UnitySwiftPlugin shared] getCurrentMousePosition];
        MousePosition vec2;
        vec2.x = (float)point.x;
        vec2.y = (float)point.y;
        return vec2;
    }

}
