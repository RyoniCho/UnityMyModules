//
//  UnitySwiftPluginBridge.h
//  ControlRoomIOSPlugins
//
//  Created by 조을연 on 2022/07/19.
//

#ifndef UnitySwiftPluginBridge_h
#define UnitySwiftPluginBridge_h

extern "C"
{
    typedef void (*DelegateCallbackFunction)(long methodId);
    typedef void (*DelegateBooleanCallbackFunction)(long methodId, bool status);
    
}

#endif /* UnitySwiftPluginBridge_h */
