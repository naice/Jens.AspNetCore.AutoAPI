/**
 * Server
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.0
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */
import { Cast } from './cast';
import { Pagination } from './pagination';
import { Sorting } from './sorting';


export interface CastQueryResponse { 
    success: boolean;
    message?: string;
    data: Array<Cast>;
    sorting: Sorting;
    pagination: Pagination;
    filter?: string;
}

