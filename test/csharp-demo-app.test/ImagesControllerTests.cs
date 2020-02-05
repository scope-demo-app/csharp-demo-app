using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace csharp_demo_app.test
{
    public class ImagesControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        private readonly Guid _restaurantId = Guid.Parse("03d207b0-8015-4ab8-950b-8155b87e1654");
        private readonly string _imageContentType = "image/gif";
        private readonly string _imageBase64 =
            "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";

        public ImagesControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Should_Get_All_Restaurant_Images()
        {
            var url = $"/images/restaurant/{_restaurantId}";
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", 
                response.Content.Headers.ContentType.ToString());
        }
        
        [Fact]
        public async Task Should_Return_Bad_Request_On_Get_All_Restaurant_Images_With_Bad_RestaurantId()
        {
            var url = $"/images/restaurant/abcde";
            var response = await _client.GetAsync(url);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        [Fact]
        public async Task Should_Return_Empty_On_Get_All_Restaurant_Images_With_Empty_RestaurantId()
        {
            var url = $"/images/restaurant/{Guid.Empty}";
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var respStr = await response.Content.ReadAsStringAsync();
            var respObj = JsonSerializer.Deserialize<string[]>(respStr);
            Assert.NotNull(respObj);
            Assert.Empty(respObj);
        }

        [Fact]
        public async Task Should_Post_Image_To_Restaurant()
        {
            var url = $"/images/restaurant/{_restaurantId}";
            var imgData = Convert.FromBase64String(_imageBase64);
            var content = new ByteArrayContent(imgData);
            content.Headers.ContentType = new MediaTypeHeaderValue(_imageContentType);

            var response = await _client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", 
                response.Content.Headers.ContentType.ToString());
            
            var responseString = await response.Content.ReadAsStringAsync();
            var imageId = JsonSerializer.Deserialize<Guid>(responseString);

            Assert.NotEqual(Guid.Empty, imageId);

            var imgUrl = $"/images/{imageId}";
            var imgResponse = await _client.GetAsync(imgUrl);
            imgResponse.EnsureSuccessStatusCode();
            
            Assert.Equal(imgResponse.Content.Headers.ContentType.MediaType, _imageContentType);
            var imgResponseData = await imgResponse.Content.ReadAsByteArrayAsync();
            
            if (!imgData.SequenceEqual(imgResponseData))
                Assert.True(false, "The image data is not the same.");
        }

        
        [Fact]
        public async Task Should_Throw_Exception_On_Post_Empty_Image_To_Restaurant()
        {
            var url = $"/images/restaurant/{_restaurantId}";
            var content = new ByteArrayContent(new byte[0]);
            content.Headers.ContentType = new MediaTypeHeaderValue(_imageContentType);

            Exception exception = null;
            try
            {
                var response = await _client.PostAsync(url, content);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            
            Assert.NotNull(exception);
        }
        
        [Fact]
        public async Task Should_Get_Images_From_Restaurant()
        {
            var url = $"/images/restaurant/{_restaurantId}";
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var respStr = await response.Content.ReadAsStringAsync();
            var respObj = JsonSerializer.Deserialize<string[]>(respStr);
            Assert.NotNull(respObj);

            foreach (var imageId in respObj)
            {
                var imgUrl = $"/images/{imageId}";
                var imgResponse = await _client.GetAsync(imgUrl);
                imgResponse.EnsureSuccessStatusCode();
            
                Assert.Equal(imgResponse.Content.Headers.ContentType.MediaType, _imageContentType);
                Assert.NotNull(imgResponse.Content.Headers.ContentLength);
                Assert.NotEqual(0, imgResponse.Content.Headers.ContentLength);
            }
        }
        
        [Fact]
        public async Task Should_Return_Bad_Request_On_Get_Image_With_Bad_ImageId()
        {
            var url = $"/images/abcde";
            var response = await _client.GetAsync(url);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        [Fact]
        public async Task Should_Return_Not_Found_On_Get_Image_With_Empty_ImageId()
        {
            var url = $"/images/{Guid.Empty}";
            var response = await _client.GetAsync(url);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        [Fact]
        public async Task Should_Remove_An_Image_From_Restaurant()
        {
            var url = $"/images/restaurant/{_restaurantId}";
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var respStr = await response.Content.ReadAsStringAsync();
            var respObj = JsonSerializer.Deserialize<string[]>(respStr);
            Assert.NotNull(respObj);

            var imageId = respObj.FirstOrDefault();
            if (imageId != null)
            {
                var imgUrl = $"/images/{imageId}";
                var imgResponse = await _client.DeleteAsync(imgUrl);
                imgResponse.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task Should_Return_Bad_Request_On_Delete_Image_With_Bad_ImageId()
        {
            var url = $"/images/abcde";
            var response = await _client.DeleteAsync(url);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        [Fact]
        public async Task Should_Return_Not_Found_On_Delete_Image_With_Empty_ImageId()
        {
            var url = $"/images/{Guid.Empty}";
            var response = await _client.DeleteAsync(url);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Should_Return_Bad_Request_On_Bad_MimeType()
        {
            var url = $"/images/restaurant/{_restaurantId}";
            var imgData = Convert.FromBase64String(_imageBase64);
            var content = new ByteArrayContent(imgData);
            content.Headers.ContentType = new MediaTypeHeaderValue("images/bad-mime");

            var response = await _client.PostAsync(url, content);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(Skip = "TODO: This test is not implemented")]
        public void Should_Return_Bad_Request_On_Post_Too_Big_Image()
        {
        }

        private readonly Guid _temporalGuid = Guid.NewGuid();
        [Fact]
        public async Task Should_Have_Exact_Four_Images()
        {
            var url = $"/images/restaurant/{_temporalGuid}";
            var imgData = Convert.FromBase64String(_imageBase64);
            var content = new ByteArrayContent(imgData);
            content.Headers.ContentType = new MediaTypeHeaderValue(_imageContentType);

            var postResponse = await _client.PostAsync(url, content);
            postResponse.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", 
                postResponse.Content.Headers.ContentType.ToString());
            
            var responseString = await postResponse.Content.ReadAsStringAsync();
            var imageId = JsonSerializer.Deserialize<Guid>(responseString);

            Assert.NotEqual(Guid.Empty, imageId);
            
            var getResponse = await _client.GetAsync(url);
            getResponse.EnsureSuccessStatusCode();
            var respStr = await getResponse.Content.ReadAsStringAsync();
            var respObj = JsonSerializer.Deserialize<string[]>(respStr);
            Assert.Equal(4, respObj.Length);
        }
        

        [Theory]
        [MemberData(nameof(RestaurantIdData))]
        public async Task Should_Return_Empty_On_Get_All_Restaurant_Images_With_Unknown_RestaurantId(string restaurantId)
        {
            var url = $"/images/restaurant/{Guid.Parse(restaurantId)}";
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var respStr = await response.Content.ReadAsStringAsync();
            var respObj = JsonSerializer.Deserialize<string[]>(respStr);
            Assert.NotNull(respObj);
            Assert.Empty(respObj);
        }

        public static IEnumerable<object[]> RestaurantIdData()
        {
            return new[]
            {
                new object[] { "f452b6a0-d917-4c8d-9f2a-7b36695df000" },
                new object[] { "f452b6a0-d917-4c8d-9f2a-7b36695df001" },
                new object[] { "f452b6a0-d917-4c8d-9f2a-7b36695df002" },
                new object[] { "f452b6a0-d917-4c8d-9f2a-7b36695df003" },
                new object[] { "f452b6a0-d917-4c8d-9f2a-7b36695df004" },
                new object[] { "f452b6a0-d917-4c8d-9f2a-7b36695df005" },
                new object[] { "f452b6a0-d917-4c8d-9f2a-7b36695df006" },
                new object[] { "f452b6a0-d917-4c8d-9f2a-7b36695df007" },
                new object[] { "f452b6a0-d917-4c8d-9f2a-7b36695df008" },
                new object[] { "f452b6a0-d917-4c8d-9f2a-7b36695df009" },
            };
        }
    }
}