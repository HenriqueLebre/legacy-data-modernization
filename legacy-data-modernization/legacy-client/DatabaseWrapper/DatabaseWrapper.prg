/*
 * DatabaseWrapper.prg - Emulates DBF commands using REST API
 */

#include "hbclass.ch"

STATIC s_cApiUrl     := ""
STATIC s_oHttpClient := NIL
STATIC s_hWorkAreas  := {=>}
STATIC s_cCurrentAlias := ""
STATIC s_lInitialized := .F.

STATIC s_hEndpoints := { ;
   "CUSTOMERS" => "/api/customers", ;
   "CLIENTES"  => "/api/customers", ;
   "PRODUCTS"  => "/api/products", ;
   "PRODUTOS"  => "/api/products" ;
}

// Initialization
FUNCTION DbWrapperInit( cApiUrl )
   s_cApiUrl := IIF( Empty( cApiUrl ), "http://localhost:5000", cApiUrl )
   s_oHttpClient := HttpClient():New( s_cApiUrl )
   IF s_oHttpClient:oWinHttp == NIL
      RETURN .F.
   ENDIF
   s_lInitialized := .T.
   RETURN .T.

FUNCTION DbWrapperIsInitialized()
   RETURN s_lInitialized

// Work Area Class
CLASS ApiWorkArea

   DATA cAlias        INIT ""
   DATA cEndpoint     INIT ""
   DATA aRecords      INIT {}
   DATA nRecNo        INIT 0
   DATA nRecCount     INIT 0
   DATA lBof          INIT .T.
   DATA lEof          INIT .T.
   DATA lFound        INIT .F.
   DATA hCurrentRecord INIT {=>}
   DATA lModified     INIT .F.

   METHOD New( cAlias, cTableName )
   METHOD Open()
   METHOD Close()
   METHOD GoTop()
   METHOD GoBottom()
   METHOD Skip( nRecords )
   METHOD Seek( xValue )
   METHOD Append()
   METHOD Replace( cField, xValue )
   METHOD Delete()
   METHOD Commit()
   METHOD FieldGet( cField )
   METHOD Eof()
   METHOD Bof()
   METHOD Found()
   METHOD RecNo()
   METHOD RecCount()

ENDCLASS

METHOD New( cAlias, cTableName ) CLASS ApiWorkArea
   ::cAlias := Upper( cAlias )
   IF HHasKey( s_hEndpoints, Upper( cTableName ) )
      ::cEndpoint := s_hEndpoints[ Upper( cTableName ) ]
   ELSE
      ::cEndpoint := "/api/" + Lower( cTableName )
   ENDIF
   RETURN Self

METHOD Open() CLASS ApiWorkArea
   LOCAL cResponse := s_oHttpClient:Get( ::cEndpoint )
   IF !Empty( cResponse ) .AND. At( "error", Lower( cResponse ) ) == 0
      ::aRecords := JsonToArray( cResponse )
      ::nRecCount := Len( ::aRecords )
   ENDIF
   ::GoTop()
   RETURN .T.

METHOD Close() CLASS ApiWorkArea
   IF ::lModified
      ::Commit()
   ENDIF
   ::aRecords := {}
   ::nRecNo := 0
   ::nRecCount := 0
   RETURN .T.

METHOD GoTop() CLASS ApiWorkArea
   IF ::nRecCount == 0
      ::nRecNo := 0
      ::lBof := .T.
      ::lEof := .T.
      ::hCurrentRecord := {=>}
   ELSE
      ::nRecNo := 1
      ::lBof := .F.
      ::lEof := .F.
      ::hCurrentRecord := ::aRecords[ 1 ]
   ENDIF
   RETURN NIL

METHOD GoBottom() CLASS ApiWorkArea
   IF ::nRecCount == 0
      ::nRecNo := 0
      ::lBof := .T.
      ::lEof := .T.
   ELSE
      ::nRecNo := ::nRecCount
      ::lBof := .F.
      ::lEof := .F.
      ::hCurrentRecord := ::aRecords[ ::nRecCount ]
   ENDIF
   RETURN NIL

METHOD Skip( nRecords ) CLASS ApiWorkArea
   LOCAL nNewPos
   DEFAULT nRecords TO 1
   nNewPos := ::nRecNo + nRecords
   IF nNewPos < 1
      ::nRecNo := 0
      ::lBof := .T.
   ELSEIF nNewPos > ::nRecCount
      ::nRecNo := ::nRecCount + 1
      ::lEof := .T.
   ELSE
      ::nRecNo := nNewPos
      ::lBof := .F.
      ::lEof := .F.
      ::hCurrentRecord := ::aRecords[ nNewPos ]
   ENDIF
   RETURN NIL

METHOD Seek( xValue ) CLASS ApiWorkArea
   LOCAL i, cResponse
   ::lFound := .F.
   
   // Try API search first
   IF ValType( xValue ) == "C"
      cResponse := s_oHttpClient:Get( ::cEndpoint + "/code/" + AllTrim( xValue ) )
      IF !Empty( cResponse ) .AND. At( "notFound", Lower( cResponse ) ) == 0
         ::hCurrentRecord := JsonToHash( cResponse )
         ::lFound := .T.
         ::lEof := .F.
         RETURN .T.
      ENDIF
   ENDIF
   
   // Local search
   FOR i := 1 TO ::nRecCount
      IF HHasKey( ::aRecords[i], "code" )
         IF Upper( AllTrim( ::aRecords[i]["code"] ) ) == Upper( AllTrim( xValue ) )
            ::nRecNo := i
            ::hCurrentRecord := ::aRecords[i]
            ::lFound := .T.
            ::lEof := .F.
            RETURN .T.
         ENDIF
      ENDIF
   NEXT
   
   ::lEof := .T.
   RETURN .F.

METHOD Append() CLASS ApiWorkArea
   LOCAL hNew := {=>}
   hNew["id"] := 0
   hNew["_isNew"] := .T.
   AAdd( ::aRecords, hNew )
   ::nRecCount := Len( ::aRecords )
   ::nRecNo := ::nRecCount
   ::hCurrentRecord := hNew
   ::lModified := .T.
   RETURN .T.

METHOD Replace( cField, xValue ) CLASS ApiWorkArea
   LOCAL cApiField := MapFieldName( cField )
   ::hCurrentRecord[ cApiField ] := xValue
   ::lModified := .T.
   RETURN .T.

METHOD Delete() CLASS ApiWorkArea
   LOCAL nId
   IF HHasKey( ::hCurrentRecord, "id" )
      nId := ::hCurrentRecord["id"]
      IF nId > 0
         s_oHttpClient:Delete( ::cEndpoint + "/" + AllTrim( Str( nId ) ) )
      ENDIF
   ENDIF
   RETURN .T.

METHOD Commit() CLASS ApiWorkArea
   LOCAL cJson, cResponse, nId
   IF !::lModified
      RETURN .T.
   ENDIF
   cJson := HashToApiJson( ::hCurrentRecord )
   IF HHasKey( ::hCurrentRecord, "_isNew" ) .AND. ::hCurrentRecord["_isNew"]
      cResponse := s_oHttpClient:Post( ::cEndpoint, cJson )
   ELSE
      nId := ::hCurrentRecord["id"]
      cResponse := s_oHttpClient:Put( ::cEndpoint + "/" + AllTrim( Str( nId ) ), cJson )
   ENDIF
   ::lModified := .F.
   RETURN .T.

METHOD FieldGet( cField ) CLASS ApiWorkArea
   LOCAL cApiField := MapFieldName( cField )
   IF HHasKey( ::hCurrentRecord, cApiField )
      RETURN ::hCurrentRecord[ cApiField ]
   ENDIF
   RETURN NIL

METHOD Eof() CLASS ApiWorkArea
   RETURN ::lEof

METHOD Bof() CLASS ApiWorkArea
   RETURN ::lBof

METHOD Found() CLASS ApiWorkArea
   RETURN ::lFound

METHOD RecNo() CLASS ApiWorkArea
   RETURN ::nRecNo

METHOD RecCount() CLASS ApiWorkArea
   RETURN ::nRecCount

// Command Functions
FUNCTION DbUseApi( cTableName, cAlias, lNew )
   LOCAL oWA
   DEFAULT cAlias TO cTableName
   DEFAULT lNew TO .T.
   cAlias := Upper( cAlias )
   oWA := ApiWorkArea():New( cAlias, cTableName )
   oWA:Open()
   s_hWorkAreas[ cAlias ] := oWA
   s_cCurrentAlias := cAlias
   RETURN .T.

FUNCTION DbCloseApi( cAlias )
   DEFAULT cAlias TO s_cCurrentAlias
   cAlias := Upper( cAlias )
   IF HHasKey( s_hWorkAreas, cAlias )
      s_hWorkAreas[ cAlias ]:Close()
      HDel( s_hWorkAreas, cAlias )
   ENDIF
   RETURN .T.

FUNCTION DbCloseAllApi()
   LOCAL cAlias
   FOR EACH cAlias IN HGetKeys( s_hWorkAreas )
      DbCloseApi( cAlias )
   NEXT
   RETURN .T.

FUNCTION GetCurrentWorkArea()
   IF HHasKey( s_hWorkAreas, s_cCurrentAlias )
      RETURN s_hWorkAreas[ s_cCurrentAlias ]
   ENDIF
   RETURN NIL

FUNCTION GetWorkArea( cAlias )
   cAlias := Upper( cAlias )
   IF HHasKey( s_hWorkAreas, cAlias )
      RETURN s_hWorkAreas[ cAlias ]
   ENDIF
   RETURN NIL

FUNCTION FIELD( cAlias, cField )
   LOCAL oWA := GetWorkArea( cAlias )
   IF oWA != NIL
      RETURN oWA:FieldGet( cField )
   ENDIF
   RETURN NIL

// Field name mapping
STATIC FUNCTION MapFieldName( cField )
   LOCAL cUpper := Upper( cField )
   DO CASE
      CASE cUpper == "CODIGO" .OR. cUpper == "COD"
         RETURN "code"
      CASE cUpper == "NOME" .OR. cUpper == "RAZAO"
         RETURN "name"
      CASE cUpper == "TELEFONE" .OR. cUpper == "FONE"
         RETURN "phone"
      CASE cUpper == "ENDERECO"
         RETURN "address"
      CASE cUpper == "CIDADE"
         RETURN "city"
      CASE cUpper == "UF" .OR. cUpper == "ESTADO"
         RETURN "state"
      CASE cUpper == "CEP"
         RETURN "zipCode"
      CASE cUpper == "EMAIL"
         RETURN "email"
      CASE cUpper == "LIMITE"
         RETURN "creditLimit"
   ENDCASE
   RETURN Lower( cField )

STATIC FUNCTION HashToApiJson( hData )
   LOCAL cJson := "{", cKey, xVal, lFirst := .T.
   FOR EACH cKey IN HGetKeys( hData )
      IF Left( cKey, 1 ) == "_" .OR. ( cKey == "id" .AND. hData[ cKey ] == 0 )
         LOOP
      ENDIF
      IF !lFirst
         cJson += ","
      ENDIF
      lFirst := .F.
      xVal := hData[ cKey ]
      cJson += '"' + cKey + '":'
      IF ValType( xVal ) == "C"
         cJson += '"' + xVal + '"'
      ELSEIF ValType( xVal ) == "N"
         cJson += AllTrim( Str( xVal ) )
      ELSEIF ValType( xVal ) == "L"
         cJson += IIF( xVal, "true", "false" )
      ELSE
         cJson += "null"
      ENDIF
   NEXT
   RETURN cJson + "}"

STATIC FUNCTION HHasKey( hHash, cKey )
   LOCAL aKeys := HGetKeys( hHash ), i
   FOR i := 1 TO Len( aKeys )
      IF aKeys[i] == cKey
         RETURN .T.
      ENDIF
   NEXT
   RETURN .F.

STATIC FUNCTION HDel( hHash, cKey )
   IF HHasKey( hHash, cKey )
      HB_HDel( hHash, cKey )
   ENDIF
   RETURN .T.

FUNCTION JsonToHash( cJson )
   // Simplified parser - production should use proper JSON library
   LOCAL hResult := {=>}
   // Basic implementation for demo
   RETURN hResult

FUNCTION JsonToArray( cJson )
   LOCAL aResult := {}
   // Basic implementation for demo
   RETURN aResult

FUNCTION HGetKeys( hHash )
   LOCAL aKeys := {}, i
   FOR i := 1 TO Len( hHash )
      AAdd( aKeys, HB_HKeyAt( hHash, i ) )
   NEXT
   RETURN aKeys
