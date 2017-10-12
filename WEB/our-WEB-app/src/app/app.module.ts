import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { HeaderComponent } from './header/header.component';

import { FooterComponent } from './footer/footer.component';
import { MainpageComponent } from './mainpage/mainpage.component';
import { SearchComponent } from './mainpage/search/search.component';
import { ShoppinglistComponent } from './mainpage/shoppinglist/shoppinglist.component';
import { ResultpageComponent } from './resultpage/resultpage.component';
import { RouterModule, Routes } from '@angular/router';

const appRouter : Routes = [
{ path : 'mainpage', component: MainpageComponent},
{ path : 'resultpage', component: ResultpageComponent},
{ path : '', redirectTo: 'mainpage', pathMatch: 'full'},
{ path: '**', redirectTo: 'mainpage', pathMatch: 'full'}
]

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    FooterComponent,
    MainpageComponent,
    SearchComponent,
    ShoppinglistComponent,
    ResultpageComponent
  ],
  imports: [
    BrowserModule,
    RouterModule.forRoot(appRouter)
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
