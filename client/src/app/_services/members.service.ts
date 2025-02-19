import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { inject, Injectable, model, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { AccountService } from './account.service';
import { of, tap } from 'rxjs';
import { Photo } from '../_models/photo';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { setPaginatedResponse, setPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  private http = inject(HttpClient);
  private accountService = inject(AccountService)

  baseUrl = environment.apiUrl
  memberCache = new Map();
  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);
  user = this.accountService.currentUser()
  userParams = signal<UserParams>(new UserParams(this.user))

  resetUserParams() {
    this.userParams.set(new UserParams(this.user))
  }

  private SetFilterHeaders(params:HttpParams, userParams: UserParams): HttpParams {
    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);
    return params;
  }

  getMembers() {
    const response = this.memberCache.get(Object.values(this.userParams()).join('-'));
    if (response) return setPaginatedResponse(response, this.paginatedResult);

    let params = setPaginationHeaders(this.userParams().pageNumber, this.userParams().pageSize);
    params = this.SetFilterHeaders(params, this.userParams())



    return this.http.get<Member[]>(this.baseUrl + 'users', { observe: 'response', params })
      .subscribe({
        next: response => {
          setPaginatedResponse(response, this.paginatedResult);
          this.memberCache.set(Object.values(this.userParams()).join('-'), response)
        }
      })
  }



  getMember(username: string) {
    const member: Member = [...this.memberCache.values()]
    .reduce((arr, elem)=>arr.concat(elem.body), [])
    .find((m: Member)=>m.username === username)

    if (member) return of(member)
    console.log(member)
    return this.http.get<Member>(this.baseUrl + 'users/' + username)
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      // tap(()=>{
      //   this.members.update(members => members.map(m=>m.username===member.username ? member : m))
      // })
    )
  }

  setMainPhoto(photo: Photo) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photo.id, {}).pipe(
      // tap(() => {
      //   this.members.update(members => members.map(m => {
      //     if (m.photos.includes(photo)) {
      //       m.photoUrl = photo.url
      //     }
      //     return m;
      //   }))
      // })
    )
  }

  deletePhoto(photo: Photo) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photo.id).pipe(
      // tap(() => {
      //   this.members.update(members => members.map(m=>{
      //     if (m.photos.includes(photo)){
      //       m.photos = m.photos.filter(x => x.id !== photo.id)
      //     }
      //     return m;
      //   }))
      // })
    )
  }
}
