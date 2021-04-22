# json-config-transform

**json-config-transform** provides command for json file transformations.

The tool provides these features :
- Applying JsonPatch transformation to a json file.

## Installation

The tool is available on [Nuget](https://www.nuget.org/packages/json-config-transform).

You can install the tool by executing this command :

```bash
$ dotnet tool install json-config-transform --global
```

## Help

Use this command to list all available commands.

```bash
$ json-config-transform --help
```

## Usages

Choose a json file to transform :

```json
// myfile.json

{
  "name": "test",
  "description": "description",
  "properties": {
    "annotations": [],
    "runtimeState": "Started",
    "pipelines": [
      {
        "pipelineReference": {
          "referenceName": "reference",
          "type": "PipelineReference"
        },
        "parameters": {
          "parameter1": "value1",
          "parameter2": "value2"
        }
      }
    ]
  }
}
```

Create a configuration file which stores all transformations (list of JSON Patch operations) like this : **operation : path => value**

```
# myfile.json.config

add : /properties/pipelines/0/parameters/parameter3 => { "p1": "value1", "p2": "value2"}
replace : /properties/pipelines/0/parameters/parameter2 => value2updated
remove : /description =>
```

Execute the tool with the following parameters :

``` bash
$ json-config-transform --source myfile.json --config myfile.json.config --display-result
```

After the execution, the json file should be like this :
- description property has been removed
- new parameter3 has been added
- parameter2 value has been updated

```json
// myfile.json

{
	"name": "test",
	"properties": {
		"annotations": [],
		"runtimeState": "Started",
		"pipelines": [
			{
				"pipelineReference": {
					"referenceName": "reference",
					"type": "PipelineReference"
				},
				"parameters": {
					"parameter1": "value1",
					"parameter2": "value2updated",
					"parameter3": {
						"p1": "value1",
						"p2": "value2"
					}
				}
			}
		]
	}
}
```

The display-result option allows to display the result of the transformation directly in the command window :

``` bash
    _                                        __ _             _                        __
   (_)___  ___  _ __         ___ ___  _ __  / _(_) __ _      | |_ _ __ __ _ _ __  ___ / _| ___  _ __ _ __ ___
   | / __|/ _ \| '_ \ _____ / __/ _ \| '_ \| |_| |/ _` |_____| __| '__/ _` | '_ \/ __| |_ / _ \| '__| '_ ` _ \
   | \__ \ (_) | | | |_____| (_| (_) | | | |  _| | (_| |_____| |_| | | (_| | | | \__ \  _| (_) | |  | | | | | |
  _/ |___/\___/|_| |_|      \___\___/|_| |_|_| |_|\__, |      \__|_|  \__,_|_| |_|___/_|  \___/|_|  |_| |_| |_|
 |__/                                             |___/

Parameters :
  - Source File Path : d:\temp\NETFLIX_CATALOG_DAILY_TRIGGER.json
  - Config File Path : d:\temp\NETFLIX_CATALOG_DAILY_TRIGGER.json.config
Result :

{
  "name": "test",
  "properties": {
    "annotations": [],
    "runtimeState": "Started",
    "pipelines": [
      {
        "pipelineReference": {
          "referenceName": "reference",
          "type": "PipelineReference"
        },
        "parameters": {
          "parameter1": "value1",
          "parameter2": "value2updated",
          "parameter3": {
            "p1": "value1",
            "p2": "value2"
          }
        }
      }
    ]
  }
}

