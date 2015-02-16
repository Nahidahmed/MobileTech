// WARNING
// This file has been generated automatically by Xamarin Studio to
// mirror C# types. Changes in this file made by drag-connecting
// from the UI designer will be synchronized back to C#, but
// more complex manual changes may not transfer correctly.


#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>


@interface TimeEntriesViewController : UIViewController {
	UITextField *_actualTimeTextField;
	UITextField *_dateTextField;
	UITextView *_timeNotesTextView;
	UITextField *_timeTextField;
	UITextField *_timeTypeTextField;
}

@property (nonatomic, retain) IBOutlet UITextField *actualTimeTextField;

@property (nonatomic, retain) IBOutlet UITextField *dateTextField;

@property (nonatomic, retain) IBOutlet UITextView *timeNotesTextView;

@property (nonatomic, retain) IBOutlet UITextField *timeTextField;

@property (nonatomic, retain) IBOutlet UITextField *timeTypeTextField;

- (IBAction)cancelButtonTouchUpInside:(id)sender;

- (IBAction)saveButtonTouchUpInside:(id)sender;

- (IBAction)saveButton:(id)sender;

@end
