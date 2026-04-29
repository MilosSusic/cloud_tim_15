export interface SpraySchedule{
    id : string,
    fieldId : string,
    startDateTime : string,
    durationHours : number,
    pesticideType : string,
    status : number,
    createdAt : string,
    updatedAt : string | null
}
