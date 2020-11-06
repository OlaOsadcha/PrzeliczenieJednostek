;#DropDB PrzeliczenieJednostekEasyTest

#Application PrzeliczenieJednostekWin
#Application PrzeliczenieJednostekWeb

*FillForm
 User Name = Admin
*Action Log In

#IfDef PrzeliczenieJednostekWin

*Action About...
 
*Action OK

#EndIf

#IfDef PrzeliczenieJednostekWeb
*Action Navigation(My Details)
#EndIf
