// WARNING
// This file has been generated automatically by Xamarin Studio to
// mirror C# types. Changes in this file made by drag-connecting
// from the UI designer will be synchronized back to C#, but
// more complex manual changes may not transfer correctly.


#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>


@interface WoEditorViewController : UIViewController {
	UILabel *_controlNoLabel;
	UILabel *_departmentLabel;
	UILabel *_descriptionLabel;
	UILabel *_devCatLabel;
	UITextField *_faultTextField;
	UITextView *_notesTextView;
	UIButton *_OWSButtonText;
	UILabel *_owStatusLabel;
	UILabel *_reqCodeDescLabel;
	UITextField *_resultTextField;
	UITextField *_safetyTestTextField;
	UIView *_safetyTestView;
	UILabel *_urgencyLabel;
}

@property (nonatomic, retain) IBOutlet UILabel *controlNoLabel;

@property (nonatomic, retain) IBOutlet UILabel *departmentLabel;

@property (nonatomic, retain) IBOutlet UILabel *descriptionLabel;

@property (nonatomic, retain) IBOutlet UILabel *devCatLabel;

@property (nonatomic, retain) IBOutlet UITextField *faultTextField;

@property (nonatomic, retain) IBOutlet UITextView *notesTextView;

@property (nonatomic, retain) IBOutlet UIButton *OWSButtonText;

@property (nonatomic, retain) IBOutlet UILabel *owStatusLabel;

@property (nonatomic, retain) IBOutlet UILabel *reqCodeDescLabel;

@property (nonatomic, retain) IBOutlet UITextField *resultTextField;

@property (nonatomic, retain) IBOutlet UITextField *safetyTestTextField;

@property (nonatomic, retain) IBOutlet UIView *safetyTestView;

@property (nonatomic, retain) IBOutlet UILabel *urgencyLabel;

- (IBAction)OWSButtonTouchUpInside:(id)sender;

@end
