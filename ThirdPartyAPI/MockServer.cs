using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ThirdPartyAPI;

public static class MockServer
{
    private static readonly int port = 8123;

    public static void Start()
    {
        var server = WireMockServer.Start(port);

        server.AddSuccess200();

        server.AddTimeout();
        server.AddError500();
        server.AddError503();
        server.AddRetriesThen200();

        server.AddGetToken();
        server.AddWithToken();

        Console.WriteLine($"WireMock server running at {port}");
        Console.WriteLine("Press any key to stop the server");
        Console.ReadKey();
    }

    // Endpoint GET /success200
    private static void AddSuccess200(this WireMockServer server)
    {
        var uri = "/success200";

        // Success (200)
        server
            .Given(
                Request.Create()
                    .WithPath(uri)
                    .UsingGet()
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("success")
            );
    }

    // Endpoint GET /timeout
    private static void AddTimeout(this WireMockServer server)
    {
        var uri = "/timeout";

        // Timeout
        server
            .Given(
                Request.Create()
                    .WithPath(uri)
                    .UsingGet()
            )
            .RespondWith(
                Response.Create()
                    .WithDelay(30 * 1000) // 30 sec
            );
    }

    // Endpoint GET /error500
    private static void AddError500(this WireMockServer server)
    {
        var uri = "/error500";

        // Internal Server Error (500)
        server
            .Given(
                Request.Create()
                    .WithPath(uri)
                    .UsingGet()
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(500)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("3rd party API server error")
            );
    }

    // Endpoint GET /error503
    private static void AddError503(this WireMockServer server)
    {
        var uri = "/error503";

        // Service unavailable (503)
        server
            .Given(
                Request.Create()
                    .WithPath(uri)
                    .UsingGet()
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(503)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("3rd party API unavailable")
            );
    }

    // Endpoint GET /retries-then-200
    private static void AddRetriesThen200(this WireMockServer server)
    {
        var uri = "/retries-then-200";

        // Retry #1 (503)
        server
            .Given(
                Request.Create()
                    .WithPath(uri)
                    .UsingGet())
                .InScenario("WaitAndRetry")
                .WillSetStateTo("Retry1")
            .RespondWith(
                Response.Create()
                    .WithStatusCode(503)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("3rd party API unavailable")
            );

        // Retry #2 (503)
        server
            .Given(
                Request.Create()
                    .WithPath(uri)
                    .UsingGet())
                .InScenario("WaitAndRetry")
                .WhenStateIs("Retry1")
                .WillSetStateTo("Retry2")
            .RespondWith(
                Response.Create()
                    .WithStatusCode(503)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("3rd party API unavailable")
            );

        // Success (200)
        server
            .Given(
                Request.Create()
                    .WithPath(uri)
                    .UsingGet())
                .InScenario("WaitAndRetry")
                .WhenStateIs("Retry2")
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("success")
            );
    }

    // Endpoint GET /get-token
    private static void AddGetToken(this WireMockServer server)
    {
        var uri = "/get-token";

        // Get Token (200)
        server
            .Given(
                Request.Create()
                    .WithPath(uri)
                    .UsingGet()
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("valid-token")
            );
    }

    // Endpoint GET /with-token
    private static void AddWithToken(this WireMockServer server)
    {
        var uri = "/with-token";

        // Valid Token (200)
        server
            .Given(
                Request.Create()
                    .WithPath(uri)
                    .WithHeader("X-TOKEN", "valid-token")
                    .UsingGet()
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("success")
            );

        // Invalid Token (401)
        server
            .Given(
                Request.Create()
                    .WithPath(uri)
                    .UsingGet()
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(401)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("invalid token error")
            );
    }
}