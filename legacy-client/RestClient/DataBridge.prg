/*
 * DataBridge.prg - High-level data access for XHarbour
 */

#include "hbclass.ch"

CLASS LegacyDataBridge

   DATA cApiUrl      INIT ""
   DATA oHttpClient  INIT NIL
   DATA cLastError   INIT ""
   DATA lConnected   INIT .F.

   METHOD New( cApiUrl )
   METHOD TestConnection()
   METHOD GetLastError()
   
   // Customer operations
   METHOD GetCustomers()
   METHOD GetCustomerById( nId )
   METHOD GetCustomerByCode( cCode )
   METHOD SearchCustomers( cTerm )
   METHOD CreateCustomer( hCustomer )
   METHOD UpdateCustomer( nId, hCustomer )
   METHOD DeleteCustomer( nId )
   METHOD GetCustomerBalance( nId )
   METHOD UpdateCustomerBalance( nId, nAmount )
   
   // Report operations
   METHOD GetMonthlySalesReport( nYear, nMonth )
   METHOD GetTopSellingProducts( dStart, dEnd, nTop )
   METHOD GetTopCustomers( dStart, dEnd, nTop )

ENDCLASS

METHOD New( cApiUrl ) CLASS LegacyDataBridge
   ::cApiUrl := cApiUrl
   ::oHttpClient := HttpClient():New( cApiUrl )
   ::lConnected := ( ::oHttpClient:oWinHttp != NIL )
   RETURN Self

METHOD TestConnection() CLASS LegacyDataBridge
   LOCAL cResponse := ::oHttpClient:Get( "/health" )
   RETURN !Empty( cResponse ) .AND. At( "error", Lower( cResponse ) ) == 0

METHOD GetLastError() CLASS LegacyDataBridge
   RETURN ::oHttpClient:GetLastError()

METHOD GetCustomers() CLASS LegacyDataBridge
   RETURN ::oHttpClient:Get( "/api/customers" )

METHOD GetCustomerById( nId ) CLASS LegacyDataBridge
   RETURN ::oHttpClient:Get( "/api/customers/" + AllTrim( Str( nId ) ) )

METHOD GetCustomerByCode( cCode ) CLASS LegacyDataBridge
   RETURN ::oHttpClient:Get( "/api/customers/code/" + cCode )

METHOD SearchCustomers( cTerm ) CLASS LegacyDataBridge
   RETURN ::oHttpClient:Get( "/api/customers/search?q=" + cTerm )

METHOD CreateCustomer( hCustomer ) CLASS LegacyDataBridge
   RETURN ::oHttpClient:Post( "/api/customers", HashToJson( hCustomer ) )

METHOD UpdateCustomer( nId, hCustomer ) CLASS LegacyDataBridge
   RETURN ::oHttpClient:Put( "/api/customers/" + AllTrim( Str( nId ) ), HashToJson( hCustomer ) )

METHOD DeleteCustomer( nId ) CLASS LegacyDataBridge
   RETURN ::oHttpClient:Delete( "/api/customers/" + AllTrim( Str( nId ) ) )

METHOD GetCustomerBalance( nId ) CLASS LegacyDataBridge
   RETURN ::oHttpClient:Get( "/api/customers/" + AllTrim( Str( nId ) ) + "/balance" )

METHOD UpdateCustomerBalance( nId, nAmount ) CLASS LegacyDataBridge
   LOCAL cJson := '{"amount":' + AllTrim( Str( nAmount ) ) + '}'
   RETURN ::oHttpClient:Post( "/api/customers/" + AllTrim( Str( nId ) ) + "/balance", cJson )

METHOD GetMonthlySalesReport( nYear, nMonth ) CLASS LegacyDataBridge
   RETURN ::oHttpClient:Get( "/api/reports/sales/monthly/" + AllTrim( Str( nYear ) ) + "/" + AllTrim( Str( nMonth ) ) )

METHOD GetTopSellingProducts( dStart, dEnd, nTop ) CLASS LegacyDataBridge
   DEFAULT nTop TO 10
   RETURN ::oHttpClient:Get( "/api/reports/products/top-selling?startDate=" + DToS( dStart ) + "&endDate=" + DToS( dEnd ) + "&top=" + AllTrim( Str( nTop ) ) )

METHOD GetTopCustomers( dStart, dEnd, nTop ) CLASS LegacyDataBridge
   DEFAULT nTop TO 10
   RETURN ::oHttpClient:Get( "/api/reports/customers/top?startDate=" + DToS( dStart ) + "&endDate=" + DToS( dEnd ) + "&top=" + AllTrim( Str( nTop ) ) )

// Simple JSON helpers
FUNCTION HashToJson( hData )
   LOCAL cJson := "{", cKey, xVal, lFirst := .T.
   FOR EACH cKey IN HGetKeys( hData )
      xVal := hData[ cKey ]
      IF !lFirst
         cJson += ","
      ENDIF
      lFirst := .F.
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

FUNCTION HGetKeys( hHash )
   LOCAL aKeys := {}, i
   FOR i := 1 TO Len( hHash )
      AAdd( aKeys, HB_HKeyAt( hHash, i ) )
   NEXT
   RETURN aKeys
