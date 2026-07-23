import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ButtonComponent } from './button.component';

describe('ButtonComponent', () => {
  let component: ButtonComponent;
  let fixture: ComponentFixture<ButtonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ButtonComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should emit clicked event when clicked', (done) => {
    component.clicked.subscribe(() => {
      expect(true).toBe(true);
      done();
    });

    component.onClick();
  });

  it('should not emit when disabled', () => {
    component.disabled = true;
    spyOn(component.clicked, 'emit');

    component.onClick();

    expect(component.clicked.emit).not.toHaveBeenCalled();
  });

  it('should apply correct variant classes', () => {
    component.variant = 'primary';
    const classes = component.getClasses();

    expect(classes['bg-blue-600 text-white hover:bg-blue-700 focus:ring-blue-500']).toBe(true);
  });

  it('should apply correct size classes', () => {
    component.size = 'lg';
    const classes = component.getClasses();

    expect(classes['px-6 py-3 text-lg']).toBe(true);
  });

  it('should disable button when loading', () => {
    component.loading = true;
    const classes = component.getClasses();

    expect(classes['opacity-50 cursor-not-allowed']).toBe(true);
  });
});
