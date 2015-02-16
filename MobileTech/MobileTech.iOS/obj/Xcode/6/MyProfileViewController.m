// WARNING
// This file has been generated automatically by Xamarin Studio to
// mirror C# types. Changes in this file made by drag-connecting
// from the UI designer will be synchronized back to C#, but
// more complex manual changes may not transfer correctly.


#import "MyProfileViewController.h"

@implementation MyProfileViewController

@synthesize departmentBtn = _departmentBtn;
@synthesize departmentLookUpTextField = _departmentLookUpTextField;
@synthesize facilityLookUpTextField = _facilityLookUpTextField;
@synthesize selectedFacilitiesTextField = _selectedFacilitiesTextField;
@synthesize selectedWorkersTextField = _selectedWorkersTextField;
@synthesize workerLookUpTextField = _workerLookUpTextField;

- (IBAction)facilityBtnArrowTap:(id)sender {
}

- (IBAction)departmentBtnArrowTap:(id)sender {
}

- (IBAction)workerBtnArrowTap:(id)sender {
}

- (IBAction)saveFilterTouchUpInside:(id)sender {
}

- (void)dealloc {
    [_selectedDepartmentsTextField release];
    [_requiredFacilities release];
    [_requiredDepartments release];
    [_requiredDepartments release];
    [_requiredWorkers release];
    [super dealloc];
}
@end
