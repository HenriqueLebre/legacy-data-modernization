/*
 * HttpWrapper.prg - HTTP Client for XHarbour
 */

#include "hbclass.ch"

#define HTTP_GET     1
#define HTTP_POST    2
#define HTTP_PUT     3
#define HTTP_DELETE  4

CLASS HttpClient

   DATA cBaseUrl     INIT ""
   DATA nTimeout     INIT 30000
   DATA cLastError   INIT ""
   DATA nLastStatus  INIT 0
   DATA oWinHttp     INIT NIL

   METHOD New( cBaseUrl )
   METHOD Get( cEndpoint )
   METHOD Post( cEndpoint, cBody )
   METHOD Put( cEndpoint, cBody )
   METHOD Delete( cEndpoint )
   METHOD Request( nMethod, cEndpoint, cBody )
   METHOD GetLastError()
   METHOD GetLastStatus()

ENDCLASS

METHOD New( cBaseUrl ) CLASS HttpClient
   ::cBaseUrl := cBaseUrl
   TRY
      ::oWinHttp := CreateObject( "WinHttp.WinHttpRequest.5.1" )
   CATCH
      ::cLastError := "Failed to create WinHTTP object"
   END
   RETURN Self

METHOD Get( cEndpoint ) CLASS HttpClient
   RETURN ::Request( HTTP_GET, cEndpoint, NIL )

METHOD Post( cEndpoint, cBody ) CLASS HttpClient
   RETURN ::Request( HTTP_POST, cEndpoint, cBody )

METHOD Put( cEndpoint, cBody ) CLASS HttpClient
   RETURN ::Request( HTTP_PUT, cEndpoint, cBody )

METHOD Delete( cEndpoint ) CLASS HttpClient
   RETURN ::Request( HTTP_DELETE, cEndpoint, NIL )

METHOD Request( nMethod, cEndpoint, cBody ) CLASS HttpClient
   LOCAL cUrl, cMethod, cResponse
   
   ::cLastError := ""
   ::nLastStatus := 0
   cResponse := ""
   
   IF ::oWinHttp == NIL
      ::cLastError := "HTTP client not initialized"
      RETURN ""
   ENDIF
   
   cUrl := ::cBaseUrl + cEndpoint
   
   DO CASE
      CASE nMethod == HTTP_GET
         cMethod := "GET"
      CASE nMethod == HTTP_POST
         cMethod := "POST"
      CASE nMethod == HTTP_PUT
         cMethod := "PUT"
      CASE nMethod == HTTP_DELETE
         cMethod := "DELETE"
   ENDCASE
   
   TRY
      ::oWinHttp:Open( cMethod, cUrl, .F. )
      ::oWinHttp:SetRequestHeader( "Content-Type", "application/json" )
      ::oWinHttp:SetRequestHeader( "Accept", "application/json" )
      ::oWinHttp:SetTimeouts( ::nTimeout, ::nTimeout, ::nTimeout, ::nTimeout )
      
      IF cBody != NIL .AND. !Empty( cBody )
         ::oWinHttp:Send( cBody )
      ELSE
         ::oWinHttp:Send()
      ENDIF
      
      ::nLastStatus := ::oWinHttp:Status
      cResponse := ::oWinHttp:ResponseText
   CATCH oError
      ::cLastError := "HTTP Error: " + oError:Description
   END
   
   RETURN cResponse

METHOD GetLastError() CLASS HttpClient
   RETURN ::cLastError

METHOD GetLastStatus() CLASS HttpClient
   RETURN ::nLastStatus
