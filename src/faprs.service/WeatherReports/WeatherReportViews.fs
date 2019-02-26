namespace WeatherReport

module Views =
    open Microsoft.AspNetCore.Http
    open Giraffe.GiraffeViewEngine

    let index = 
        let content = [
            encodedText "It's the Tucson Snowpocaplyse 2019"
        ]
        App.layout content