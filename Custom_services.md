# Custom services

the customs services are use to extend the feature of the Sdk.
for inject data in the service, you can

* Add a property in the model and declare the value with '$' prefix.   
In this sample the argument "$argumentName" describe the source

```JSON
{ "result" : { "$type":"name of service", "$argumentName": "<value>"  } }
``` 

* Consume the flow of data.  
Jpath select a value and the function distinct return false if the value is already matched.
```JSON
{ "$where" : "jpath:{$.property_Path} | distinct:{}" }
```

* Consume the flow of data.  
Jpath select a value and the function notnul return false if the value is null.
```JSON
{ "$where" : "jpath:{$.property_Path} | notnull:{}" }
```


## Custom services embedded in the Sdk
## **Add**  
Add the arguments. Note the values must be numeric (integer or float)
```JSON
"Syntax" : { "$type" : "add", "$left":"<value>", "$right":"$value" }
```

## **Concat**  
Concat Arguments of type text. Note all arguments are not required
```JSON
"Syntax" : { "$type" : "concat", 
    "$arg1":"<value>", 
    "$arg2":"$value",
    "$arg3":"$value",
    "$arg4":"$value"
}
```

## **ConcatAll**  
Concat input stream of type text. 
```JSON
["jpath:{$[0].Text}", { "$type" : "concat", "$splitchar":",", "$delimitchar":"\" }"]
```

## **Crc32**  
Compute Crc32 checksum  
```JSON
"Syntax" : { "$type" : "crc32" }
```

## **Generate id**  
format text and Compute Crc32 checksum  
```JSON
"Syntax" : { "$type" : "id" }
```

## **Distinct**  
return false in the value is already matched in the input list
```JSON
"Syntax" : { "$type" : "distinct" }
```

## **Div**  
return the result of the division
```JSON
"Syntax" : { "$type" : "div", "$left":"<value>", "$right":"$value" }
```

## **Format**  
return a formatted text

if the value is integer the formatting documentation is [here](Format_integer.md)  
if the value is double the formatting documentation is [here](Format_double.md)  
if the value is datetime the formatting documentation is [here](Format_DateTime.md)  
if the value is guid the formatting documentation is [here](Format_Guid.md)  
if the value is timespan the formatting documentation is [here](Format_Timespan.md)  

```JSON
"Syntax" : { "$type" : "format", "$mask" : "G", "$culture" : "fr-FR" }
```

## **Frombase64**  
return text of value encoded in base 64  
```JSON
"Syntax" : { "$type" : "frombase64" }
```

## **Modulo**  
return the rest of the division
```JSON
"Syntax" : { "$type" : "modulo", "$left":"<value>", "$right":"$value" }
```

## **Now**  
return the current date and time  
```JSON
[{ "$type" : "utc", "utc":true }"]
```

## **Sha256**  
return the hash of Sha256 algorithm  
```JSON
"Syntax" : { "$type" : "sha256" }
```

## **Sha512**  
return the hash of Sha512 algorithm
```JSON
"Syntax" : { "$type" : "sha512" }
```

## **SubStr**  
return a sub string of text
```JSON
"Syntax" : { "$type" : "substr", "$text":"<value>", "$start":"$value", "$length":"$value" }
```

## **Subtract**  
return the result of the subtraction  
```JSON
"Syntax" : { "$type" : "substract", "$left":"<value>", "$right":"$value" }
```

## **Sum**  
return the sum of array of numeric
```JSON
"Syntax" : { "$type" : "sum" }
```

## **Time**  
return the result of multiplication
```JSON
"Syntax" : { "$type" : "time", "$left":"<value>", "$right":"$value" }
```

## **ToBase64**  
return a text encoded base 64
```JSON
"Syntax" : { "$type" : "tobase64" }
```


