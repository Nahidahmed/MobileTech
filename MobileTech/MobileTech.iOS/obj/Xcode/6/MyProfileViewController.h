// WARNING
// This file has been generated automatically by Xamarin Studio to
// mirror C# types. Changes in this file made by drag-connecting
// from the UI designer will be synchronized back to C#, but
// more complex manual changes may not transfer correctly.


#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>


@interface MyProfileViewController : UIViewController {
	UIButton *_departmentBtn;
	UITextField *_departmentLookUpTextField;
	UITextField *_facilityLookUpTextField;
	UITextField *_selectedFacilitiesTextField;
	UITextField *_selectedWorkersTextField;
	UITextField *_workerLookUpTextField;
}

@property (nonatomic, retain) IBOutlet UIButton *departmentBtn;

@property (nonatomic, retain) IBOutlet UITextField *departmentLookUpTextField;

@property (nonatomic, retain) IBOutlet UITextField *facilityLookUpTextField;

@property (nonatomic, retain) IBOutlet UITextField *selectedFacilitiesTextField;

@property (nonatomic, retain) IBOutlet UITextField *selectedWorkersTextField;

@property (nonatomic, retain) IBOutlet UITextField *workerLookUpTextField;

- (IBAction)facilityBtnArrowTap:(id)sender;

- (IBAction)departmentBtnArrowTap:(id)sender;

- (IBAction)workerBtnArrowTap:(id)sender;

- (IBAction)saveFilterTouchUpInside:(id)sender;
@property (retain, nonatomic) IBOutlet UITextField *selectedDepartmentsTextField;
@property (retain, nonatomic) IBOutlet UILabel *requiredFacilities;

@property (retain, nonatomic) IBOutlet UILabel *requiredDepartments;
@property (retain, nonatomic) IBOutlet UILabel *requiredWorkers;

@end
