# **Entrance Test Submission (.NET)**
This is my entrance test submission for the Dot Net Entrance Evaluation.
## Directory structure
- **Controllers**: Handles HTTP requests and responses. Defines API endpoints.
- **Data**: Contains database-related code to manage DbContext class to manage database connection and schemas.
- **DTOs**: Used to transfer data between client and server or between layers of the application.
- **Models**: Defines entities representing tables in the database.
- **Repositories**: Implements data access logics and operations.
- **Services**: Contains JWT service to handle the token generation logic.

## appsetting.json folder**
Please add the below line of code inside the appsetting.json in order to make the application work (replace the information inside the "{}").

    "ConnectionStrings": {
        "DefaultConnection": "server={IP};port={your_port};database={your_db};user={your_username};password={your_password};"
    },
    "Jwt": {
        "Secret": "{yoursecretkey}",
        "Issuer": "NETAPI",
        "Audience": "NETAPIClient",
        "AccessTokenExpr": "60"
    }
