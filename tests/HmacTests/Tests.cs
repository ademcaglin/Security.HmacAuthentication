using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace HmacTests
{
    public class Tests
    {
        private const string _correctApiKey = "abc670d15a584f4baf0ba48455d3b155";
        private const string _correctAppId = "jDEf7bMcJVFnqrPd599aSIbhC0IasxLBpGAJeW3Fzh4=";

        private readonly TestServer _server;
        private readonly HttpClient _client;

        public Tests()
        {
            // Arrange
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task Should_Validate()
        {
            var helper = new Helper(_correctAppId, _correctApiKey, "/validate");
            // Act
            var response = await _client.SendAsync(helper.GetRequest());

            // Assert
            Assert.Equal("True", response.Content.ReadAsStringAsync().Result);
        }
        [Fact]
        public async Task Should_Not_Validate_If_InvalidApiKey()
        {
            var helper = new Helper(_correctAppId, "dfsgfdgdfgfd", "/validate");
            // Act
            var response = await _client.SendAsync(helper.GetRequest());

            // Assert
            Assert.Equal("False", response.Content.ReadAsStringAsync().Result);
        }

        [Fact]
        public async Task Should_Not_Validate_If_InvalidAppId()
        {
            var helper = new Helper("sfsdfdfd", _correctApiKey, "/validate");
            // Act
            var response = await _client.SendAsync(helper.GetRequest());

            // Assert
            Assert.Equal("False", response.Content.ReadAsStringAsync().Result);
        }

        [Fact]
        public async Task Should_Not_Validate_If_ReplayAttack()
        {
            var helper = new Helper(_correctAppId, _correctApiKey, "/replayattack");
            // Act
            var response1 = await _client.SendAsync(helper.GetRequest("sffsdf"));
            var response2 = await _client.SendAsync(helper.GetRequest("sffsdf"));

            // Assert
            Assert.Equal("True", response1.Content.ReadAsStringAsync().Result);
            Assert.Equal("False", response2.Content.ReadAsStringAsync().Result);
        }
    }
}
