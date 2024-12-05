import { Component, inject, input, OnInit, output } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { TextInputComponent } from "../_forms/text-input/text-input.component";
import { DatePickerComponent } from '../_forms/date-picker/date-picker.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, TextInputComponent, DatePickerComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
  private accountService = inject(AccountService);
  private router = inject(Router)
  private fb = inject(FormBuilder)
  registerForm: FormGroup = new FormGroup({});
  cancelRegister = output<boolean>();
  maxDate = new Date();
  validationErrors: string[] | undefined;


  ngOnInit(): void {
    this.initializeForm()
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18)
  }

  initializeForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      username: ['Hello', Validators.required],
      password: ['', [ Validators.required, Validators.minLength(4), Validators.maxLength(8) ]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]],
    })
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    })
  }

  matchValues(matchTo:string): ValidatorFn{
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null :  { isMatching: true}
    }
  }

  register() {
    const dob = this.registerForm.get('dateOfBirth')?.value;
    this.registerForm.patchValue({dateOfBirth: this.getDbOnly(dob)})
    this.accountService.register(this.registerForm.value).subscribe({
      next: _ => this.router.navigateByUrl('/members'),
      error: error => this.validationErrors = error
    })
  }

  cancel() {
    console.log('cancelled');
    this.cancelRegister.emit(false);
  }

  private getDbOnly(dob:string | undefined) {
    if (!dob) return;
    return new Date(dob).toISOString().slice(0,10)
  }
}
