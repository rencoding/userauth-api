# UserAuthAPI
User Authentication Api

  A RESTful API for Sign In, Sign Up and Search User. Asp.Net Core 2.1 and EF Core 2. 1 are used to build the Api 

The API exposes below three end-points 

    1. Sign-up
       
       // URI: POST: api/account/signup
       E.g. https://localhost:44330/api/account/signup

       Body       
       {
          "Name":"wwwww",
          "Email":"wwwww@gmail.com",
          "Password":"12345",
          "Token":"e",
          "TelephoneNumbers":["123456789"]
       }
       
       On succesful sign-up, the below output format is expected,
       
       {
          "id": "6b463f5e-6e42-4572-88c1-418171474791",
          "createdOn": "2019-05-27T07:18:44.1094434Z",
          "lastUpdatedOn": "2019-05-27T07:18:44.3262112Z",
          "lastLoginOn": "2019-05-27T07:18:44.3262108Z",
          "token":    JWTToken
       }
   
    2. Sign-in 

	// URI : GET : /api/account/signin?email=xxxx&password=xxxx

	E.g. https://localhost:44330/api/account/signin?email=test@test.com&password=****
      
	Upon successful sign-in, a new JWT token is created. The output would look like
      
	{
	   "id": "6b463f5e-6e42-4572-88c1-418171474791",
	   "createdOn": "2019-05-27T07:18:44.11",
	   "lastUpdatedOn": "2019-05-27T07:25:30.9129246Z",
	   "lastLoginOn": "2019-05-27T07:25:30.9128601Z",
	   "token": JWTToken
	}

    3. Search User

	// URI: GET : /api/account/searchuser{id}
    
	E.g. https://localhost:44330/api/account/searchuser/6b463f5e-6e42-4572-88c1-418171474791
    
	Upon successful searching the user, the api return a user session status among below
		
	   a.Authorized
	   b.Unauthorized
	   c.InvalidSession

Source Code Structure

  Below are the folder structure

  Src

	DAL - Contains the Db models, DbContexts and Repositories
	WebAPI - Contains the Startup, Api Controllers, Domain Entities and UserService

  Tests

    UnitTest - xUnit, Moq are used
	IntegrationTest - xUnit, In-Memory database are used


Database Migration

  Project is developed using Code First Migration approach and tested with SQL Server 13.0 (LocalDb)
  Database connection string is read from the appsettings.json. SQL server users credentials may apply as it may required in the higher environments

 Below commands are used for the migration

   > Add-Migration InitialCreate
  
   > Update-Database

Other Features

Exception handling is abstracted away from the code and it is handling by an ExceptionMiddleware which is bootstrapped in the startup.cs

JWT Token is generated

Swagger UI is configured to explore and test the Api endpoints. Api end-points are tested using Postman as well as Swagger

User password is hashed using BCrypt.Net.BCrypt nuget package and stored. The secret key is used from appsettings.json

Below is the Swagger UI Url to view the end-points
  /swagger/index.html
