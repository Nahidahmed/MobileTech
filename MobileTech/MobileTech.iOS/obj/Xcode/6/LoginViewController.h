// WARNING
// This file has been generated automatically by Xamarin Studio to
// mirror C# types. Changes in this file made by drag-connecting
// from the UI designer will be synchronized back to C#, but
// more complex manual changes may not transfer correctly.


#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>


@interface LoginViewController : UIViewController {
	UIActivityIndicatorView *_activityMonitor;
	UIButton *_btnPurgeDB;
	UIButton *_btnUploadLogToServer;
	UILabel *_lblDBVersion;
	UILabel *_lblWSVersion;
	UIView *_loginView;
	UIView *_mAnimationContainer;
	UIView *_mServerSetupView;
	UITextField *_txtPassword;
	UITextField *_txtServerAddress;
	UITextField *_txtUsername;
}

@property (nonatomic, retain) IBOutlet UIActivityIndicatorView *activityMonitor;

@property (nonatomic, retain) IBOutlet UIButton *btnPurgeDB;

@property (nonatomic, retain) IBOutlet UIButton *btnUploadLogToServer;

@property (nonatomic, retain) IBOutlet UILabel *lblDBVersion;

@property (nonatomic, retain) IBOutlet UILabel *lblWSVersion;

@property (nonatomic, retain) IBOutlet UIView *loginView;

@property (nonatomic, retain) IBOutlet UIView *mAnimationContainer;

@property (nonatomic, retain) IBOutlet UIView *mServerSetupView;

@property (nonatomic, retain) IBOutlet UITextField *txtPassword;

@property (nonatomic, retain) IBOutlet UITextField *txtServerAddress;

@property (nonatomic, retain) IBOutlet UITextField *txtUsername;

- (IBAction)btnPurgeDBClicked:(id)sender;

- (IBAction)flipToServerSetup:(id)sender;

- (IBAction)btnUploadLogToServerClick:(id)sender;

- (IBAction)btnLoginAction:(id)sender;

- (IBAction)flipToLogin:(id)sender;

@end
