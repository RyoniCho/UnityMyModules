//
//  UnitySwiftPlugin.swift
//  ControlRoomIOSPlugins
//
//  Created by 조을연 on 2022/07/19.
//

import Foundation
import UIKit
import AVFoundation
import Photos
import GameController





@objc public class UnitySwiftPlugin: NSObject
{
    @objc public static let shared = UnitySwiftPlugin()
    var parentView :UIView!
    private var currentPoint: CGPoint = .zero
    
    override public init() {
        
        self.parentView = UIApplication.shared.delegate?.window??.rootViewController?.view
        
        super.init()
        
        setupPointerInteraction()
               
    }
    
    private func setupPointerInteraction() {
            if let window = UIApplication.shared.windows.first {
                if #available(iOS 13.4, *) {
                    let pointerInteraction = UIPointerInteraction(delegate: self)
                    window.addInteraction(pointerInteraction)
                }
            }
        }
    
    @objc public func getCurrentMousePosition() -> CGPoint {
            return currentPoint
        }
    

    @objc public func ShowToast(message:String)
    {
        if(self.parentView == nil)
        {
            return
        }
            
        let toastLabel = PaddingUILabel()
        
        toastLabel.backgroundColor = UIColor.black.withAlphaComponent(0.6)
        toastLabel.textColor = UIColor.white
        toastLabel.font = UIFont.systemFont(ofSize: 14.0)
        toastLabel.text = message
        toastLabel.alpha = 1.0
        toastLabel.layer.cornerRadius = 10
        toastLabel.textAlignment = NSTextAlignment.center
        toastLabel.numberOfLines = 0
        toastLabel.lineBreakMode = NSLineBreakMode.byCharWrapping

        let newSize = GetWidthSizeFromText(message: message)
        toastLabel.frame.size=newSize
        
        toastLabel.frame.origin.x = self.parentView.frame.size.width/2-(toastLabel.frame.width/2)
        toastLabel.frame.origin.y = self.parentView.frame.size.height-150
        
        toastLabel.clipsToBounds = true
       

        self.parentView.addSubview(toastLabel)
        
        UIView.animate(withDuration: 4.0, delay: 0.1, options: .curveEaseOut, animations: {
                     toastLabel.alpha = 0.0
                }, completion: {(isCompleted) in
                    toastLabel.removeFromSuperview()
                })
        
    }

    @objc public func RequestMicrophonePermission()
    {
        let currentMicPermission=AVAudioSession.sharedInstance().recordPermission
        
    
        if currentMicPermission == AVAudioSession.RecordPermission.undetermined
        {
            AVAudioSession.sharedInstance().requestRecordPermission({(granted:Bool)->Void in
                if granted{
                    print("Mic permission granted")
                }
               
            })
        }
    }

    @objc public func CanShowRequestMicPermission()->Bool
    {
         let currentMicPermission=AVAudioSession.sharedInstance().recordPermission

         if(currentMicPermission==AVAudioSession.RecordPermission.undetermined){
            return true
         }

         return false

    }

    @objc public func CheckCameraPermissionAllowed()->Bool
    {
        let cameraMediaType = AVMediaType.video
        let cameraAuthorizationStatus = AVCaptureDevice.authorizationStatus(for: cameraMediaType)
    
        if(cameraAuthorizationStatus == .authorized)
        {
            print("CheckCameraPermissionAllowed authorized")
            return true
        }

         print("CheckCameraPermissionAllowed deniedd")
        
        return false
    }

   @objc public func CheckPhotoLibraryPermissionAllowed()->Bool
    {
        
        if #available(iOS 14, *) {
            let addOnlyPermissionStatus = PHPhotoLibrary.authorizationStatus(for:.addOnly)
            
            if(addOnlyPermissionStatus == .authorized || addOnlyPermissionStatus == .limited)
            {
               
                return true
            }
            
            let fullPermissionStatus = PHPhotoLibrary.authorizationStatus()
           
            if(fullPermissionStatus == .authorized || fullPermissionStatus == .limited)
            {
                
                return true
            }
            
        }
        // Fallback on earlier versions
        else {
            
            let photoAuthorizationStatus = PHPhotoLibrary.authorizationStatus()
           
            if(photoAuthorizationStatus == .authorized)
            {
                 print("CheckPhotoLibraryPermissionAllowed authorized")
                return true
            }
        }

          print("CheckPhotoLibraryPermissionAllowed denied")
    
        return false
    }

    @objc public func OpenAppSettings()
    {
        if let url = URL.init(string: UIApplication.openSettingsURLString)
        {
            UIApplication.shared.open(url,options: [:],completionHandler: nil)
        }
    }
    
    private func GetWidthSizeFromText(message:String)->CGSize
    {
        let tempLabel=UILabel()
        tempLabel.font = UIFont.systemFont(ofSize: 14.0)
        let splitMessage = message.split(separator: "\n").map{(value)->String in return String(value)}
        
        var maxCount = 0
        var longMessage : String = ""
        
        for _message in splitMessage
        {
            if(maxCount<_message.count)
            {
                maxCount=_message.count
                longMessage=_message
            }
                
        }
        
        tempLabel.text = longMessage
        
        return CGSize(width: tempLabel.intrinsicContentSize.width+48.5,height:tempLabel.intrinsicContentSize.height+48.5)
        
    }
    
    @objc public func ShowNativeAlertPopup(methodId:Int, title:String, message:String, setCancelButton:Bool, okCallback: @escaping @convention(c) (_ methodid:Int)-> Swift.Void, cancelCallback: @escaping @convention(c) (_ methodid:Int)-> Swift.Void)
    {
        let alert = UIAlertController(title:title,message: message,preferredStyle: .alert)
        
        let defalutAction = UIAlertAction(title:"OK",style: .default){(action) in
            print("ShowNativeAlertPopup: Ok Action")
            okCallback(methodId)
        }
        
        let cancelAction = UIAlertAction(title:"Cancel",style: .default){
            (action) in
            print("ShowNativeAlertPopup: Cancel Action")
            cancelCallback(methodId)
        }
        
        alert.addAction(defalutAction)
        
        if setCancelButton == true
        {
            alert.addAction(cancelAction)
        }
       

       
        let viewController = UIApplication.shared.windows.first!.rootViewController!
        
        viewController.present(alert,animated: false,completion: nil)
        
    }
    
    
    @objc public func AddObserverGCKeyboard(methodId: Int, eventCallback: @escaping @convention(c) (_ methodid:Int, _ status:Bool)-> Swift.Void)
    {
    
        if #available(iOS 14.0, *) 
        {
            NotificationCenter.default.addObserver(forName: .GCKeyboardDidConnect, object: nil, queue: .main){_ in eventCallback(methodId,true)}
            NotificationCenter.default.addObserver(forName: .GCKeyboardDidDisconnect, object: nil, queue: .main){_ in eventCallback(methodId,false)}
        } 
        
    }
    
     @objc public func AddObserverGCController(methodId: Int, eventCallback: @escaping @convention(c) (_ methodid:Int, _ status:Bool)-> Swift.Void)
        {
        
            if #available(iOS 14.0, *) 
            {
                NotificationCenter.default.addObserver(forName: .GCControllerDidConnect, object: nil, queue: .main){_ in eventCallback(methodId,true)}
                NotificationCenter.default.addObserver(forName: .GCControllerDidDisconnect, object: nil, queue: .main){_ in eventCallback(methodId,false)}
            } 
          
        
        }
     @objc public func AddObserverGCMouse(methodId: Int, eventCallback: @escaping @convention(c) (_ methodid: Int, _ status: Bool) ->Swift.Void)
     {
             if #available(iOS 14.0, *)
             {
                 NotificationCenter.default.addObserver(forName: .GCMouseDidConnect, object: nil, queue: .main){_ in eventCallback(methodId,true)}
                 NotificationCenter.default.addObserver(forName: .GCMouseDidDisconnect, object: nil, queue: .main){_ in eventCallback(methodId,false)}
             }
               
     }

     @objc public func IsAnyExternalKeyboardConnected()->Bool
     {
        
        if #available(iOS 14.0, *) 
        {
            if GCKeyboard.coalesced != nil
            {
                return true
            }
            
        }
       
        return false
           
     }
     @objc public func IsAnyExternalGamepadConnected()->Bool
     {
             
             if #available(iOS 14.0, *) 
             {
                 if GCController.current != nil
                 {
                     return true
                 }
                 
             }
            
             return false
                
     }
     
     @objc public func IsAnyExternalMouseConnected()->Bool
         {
            
            if #available(iOS 14.0, *)
            {
                if GCMouse.current != nil
                {
                    return true
                }
               
            }
             return false
               
         }
     @objc public func GetGCMouseScrollYAxisValue() -> Float
         {
             if #available(iOS 14.0, *)
             {
                 if let gcMouse = GCMouse.current 
                 {
                     return gcMouse.mouseInput?.scroll.yAxis.value ?? 0
                 }
             }
             
             return 0
         }
     @objc public func GetGCMouseScrollXAxisValue() -> Float
        {
                 if #available(iOS 14.0, *)
                 {
                     if let gcMouse = GCMouse.current
                     {
                         return gcMouse.mouseInput?.scroll.xAxis.value ?? 0
                     }
                 }
                 
                 return 0
        }
    
    
    
     
    
    class PaddingUILabel: UILabel
    {
        override func drawText(in rect: CGRect) {
            let insets=UIEdgeInsets(top: 4.0, left: 8.0, bottom: 4.0, right: 4.0)
            super.drawText(in: rect.inset(by: insets))
        }
        
        override var intrinsicContentSize: CGSize
        {
            let size = super.intrinsicContentSize
            return CGSize(width: size.width+16.0, height: size.height+8.0)
        }
        
        override var bounds: CGRect{
            didSet{
                preferredMaxLayoutWidth=bounds.width-16.0
                
            }
        }
    }
}

extension UnitySwiftPlugin: UIPointerInteractionDelegate {
    @available(iOS 13.4, *)
    public func pointerInteraction(_ interaction: UIPointerInteraction, regionFor request: UIPointerRegionRequest, defaultRegion: UIPointerRegion) -> UIPointerRegion? {
        currentPoint = request.location
        return nil
    }
}
