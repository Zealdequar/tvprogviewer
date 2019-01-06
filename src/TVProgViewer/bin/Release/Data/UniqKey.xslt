<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
  <xsl:include href ="MergeXmltv.xslt"/>
  <xsl:key name="mergekey1" match="channel" use ="@id"/>
  <xsl:key name="mergekey2" match="programme" use ="concat(@start,@stop,@channel)"/>
   <xsl:output method="xml" indent="yes"/>
  <xsl:template name="mergekey-value1">
    <xsl:value-of select="@id"/>
  </xsl:template>
  <xsl:template name="mergekey-value2">
    <xsl:value-of select="concat(@start,@stop,@channel)"/>
  </xsl:template>
</xsl:stylesheet>